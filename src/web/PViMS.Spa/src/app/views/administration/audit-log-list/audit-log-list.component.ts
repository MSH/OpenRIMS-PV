import { Component, OnInit, ViewChild, OnDestroy, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from 'app/shared/services/event.service';
import { AccountService } from 'app/shared/services/account.service';
import { PopupService } from 'app/shared/services/popup.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { BaseComponent } from 'app/shared/base/base.component';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { _routes } from 'app/config/routes';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { UserService } from 'app/shared/services/user.service';
import { HttpEventType } from '@angular/common/http';
import { ProgressStatus, ProgressStatusEnum } from 'app/shared/models/program-status.model';
import { ExcelGenService } from 'app/shared/services/excel-gen.service';
import { FacilityIdentifierModel } from 'app/shared/models/facility/facility.identifier.model';
import { FacilityService } from 'app/shared/services/facility.service';

const moment =  _moment;

@Component({
  templateUrl: './audit-log-list.component.html',
  styleUrls: ['./audit-log-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class AuditLogListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected eventService: EventService,
    protected userService: UserService,
    protected facilityService: FacilityService,
    protected excelGenService: ExcelGenService,
    protected mediaObserver: MediaObserver,
    public accountService: AccountService) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          this.setupTable();
      }
    });    
  }

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  percentage: number;
  showProgress: boolean;

  facilityList: FacilityIdentifierModel[] = [];

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.viewModelForm = self._formBuilder.group({
      auditType: [this.viewModel.auditType || 'UserLogin', Validators.required],
      facilityId: [this.viewModel.facilityId || '0', Validators.required],
      searchFrom: [this.viewModel.searchFrom || moment().subtract(3, 'months'), Validators.required],
      searchTo: [this.viewModel.searchTo || moment(), Validators.required],
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(AuditLogListComponent.name);
  }  

  setupTable() {
    if (this.currentScreenWidth === 'xs') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['audit-type', 'created-date']);
    }

  }; 

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
  }

  getFacilityList(): void {
    let self = this;
    self.facilityService.getAllFacilities()
      .pipe(takeUntil(self._unsubscribeAll))
      .subscribe(result => {
        self.facilityList = result;
      }, error => {
        self.handleError(error, "Error fetching facility list");
      });
  }  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.userService.getAuditLogs(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching audit logs");
        });
  }

  download(model: GridRecordModel = null): void {
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.userService.downloadAuditLog(model.id).subscribe(
      data => {
        switch (data.type) {
          case HttpEventType.DownloadProgress:
            this.downloadStatus( {status: ProgressStatusEnum.IN_PROGRESS, percentage: Math.round((data.loaded / data.total) * 100)});
            break;

          case HttpEventType.Response:
            this.downloadStatus( {status: ProgressStatusEnum.COMPLETE});
            
            const downloadedFile = new Blob([data.body], { type: data.body.type });
            const a = document.createElement('a');

            a.setAttribute('style', 'display:none;');
            document.body.appendChild(a);
            a.download = '';
            a.href = URL.createObjectURL(downloadedFile);
            a.target = '_blank';
            a.click();
            document.body.removeChild(a);
            break;
        }
      },
      error => {
        this.downloadStatus( {status: ProgressStatusEnum.ERROR} );
      }
    );
  }

  public downloadStatus(event: ProgressStatus) {
    switch (event.status) {
      case ProgressStatusEnum.START:
        this.setBusy(true);
        break;

      case ProgressStatusEnum.IN_PROGRESS:
        this.showProgress = true;
        this.percentage = event.percentage;
        break;

      case ProgressStatusEnum.COMPLETE:
        this.showProgress = false;
        this.setBusy(false);
        break;

      case ProgressStatusEnum.ERROR:
        this.showProgress = false;
        this.setBusy(false);
        this.throwError('Error downloading file. Please try again.', 'Error downloading file. Please try again.');
        break;
    }
  }

  generateExcelForGrid() {
    let self = this;
    self.setBusy(true);

    self.userService.getAllAuditLogs(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
          let mappedResults = result.map(({ auditType, details, actionDate, userFullName }) => ({ auditType, details, actionDate, userFullName }));

          self.excelGenService.generateExcelForGrid('Audit Trail', ['Audit type', 'Details', 'Created', 'Username'], mappedResults);
          self.setBusy(false);
          
        }, error => {
          self.handleError(error, "Error fetching audit logs for extract");
        });
  }

  downloadDataset(): void {
    let self = this;
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.userService.downloadAuditLogDataset(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value)).subscribe(
      data => {
        switch (data.type) {
          case HttpEventType.DownloadProgress:
            this.downloadStatus( {status: ProgressStatusEnum.IN_PROGRESS, percentage: Math.round((data.loaded / data.total) * 100)});
            break;

          case HttpEventType.Response:
            this.downloadStatus( {status: ProgressStatusEnum.COMPLETE});
            
            const downloadedFile = new Blob([data.body], { type: data.body.type });
            const a = document.createElement('a');

            a.setAttribute('style', 'display:none;');
            document.body.appendChild(a);
            a.download = '';
            a.href = URL.createObjectURL(downloadedFile);
            a.target = '_blank';
            a.click();
            document.body.removeChild(a);

            this.notify("Dataset downloaded successfully!", "Success");            
            break;
        }
      },
      error => {
        this.downloadStatus( {status: ProgressStatusEnum.ERROR} );
      }
    );
  }

}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['audit-type', 'details', 'created-date', 'user', 'actions']);

  auditType: string;
  facilityId: number;
  searchFrom: Moment;
  searchTo: Moment;
}

class GridRecordModel {
  id: number;
  auditType: string;
  actionDate: string;
  details: string;
  userFullName: string;
  hasLog: boolean;
}