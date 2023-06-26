import { Component, OnInit, AfterViewInit, OnDestroy, ViewEncapsulation, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MediaObserver } from '@angular/flex-layout';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { takeUntil, finalize } from 'rxjs/operators';
import { GridModel } from 'app/shared/models/grid.model';
import { _routes } from 'app/config/routes';
import { MetaReportService } from 'app/shared/services/meta-report.service';
import { ReportConfigurePopupComponent } from '../shared/report-configure-popup/report-configure.popup.component';
import { GenericDeletePopupComponent } from '../shared/generic-delete-popup/generic-delete.popup.component';
import { AttributeConfigurePopupComponent } from './attribute-configure-popup/attribute-configure.popup.component';
import { FilterConfigurePopupComponent } from './filter-configure-popup/filter-configure.popup.component';

@Component({
  templateUrl: './report-list.component.html',
  styleUrls: ['./report-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class ReportListComponent  extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected metaReportService: MetaReportService,
    protected dialog: MatDialog,    
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);
  }

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
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
  } 

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaReportService.getMetaReportsByDetail(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching meta reports");
        });
  }

  openReportConfigurePopup(metaReportId: number) {
    let self = this;
    let title = 'Add Report';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ReportConfigurePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { metaReportId: metaReportId, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }

  openDeletePopUp(metaReportId: number, reportName: string) {
    let self = this;
    let title = 'Delete Report';
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { id: metaReportId, type: 'MetaReport', title: title, name: reportName }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }  

  openAttributePopUp(metaReportId: number, reportName: string) {
    let self = this;
    let title = 'Set Attributes';
    let dialogRef: MatDialogRef<any> = self.dialog.open(AttributeConfigurePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { metaReportId: metaReportId, title: title, reportName: reportName }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }  

  openFilterPopUp(metaReportId: number, reportName: string) {
    let self = this;
    let title = 'Set Filters';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FilterConfigurePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { metaReportId: metaReportId, title: title, reportName: reportName }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }  

}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['report-name', 'unique-identifier', 'system', 'report-status', 'actions']);
}

class GridRecordModel {
  id: number;
  metaReportGuid: string;
  reportName: string;
  reportDefinition: string;
  metaDefinition: string;
  breadCrumb: string;
  system: string;
  reportStatus: string;
}