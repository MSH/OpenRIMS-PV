import { Component, OnInit, AfterViewInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { GridModel } from 'app/shared/models/grid.model';
import { finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ProgressStatusEnum, ProgressStatus } from 'app/shared/models/program-status.model';
import { HttpEventType } from '@angular/common/http';
import { _routes } from 'app/config/routes';

@Component({
  templateUrl: './activity-history.component.html',
  styles: [`
    .mat-column-activity { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-execution-event { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-executed-by { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-executed-date { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-comments { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-receipt-date { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-receipt-code { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],  
  animations: egretAnimations
})
export class ActivityHistoryComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected reportInstanceService: ReportInstanceService,
    protected eventService: EventService,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          this.setupTable();
      }
    });
  }

  public itemForm: FormGroup;

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  percentage: number;
  showProgress: boolean;

  workFlowId: string;
  qualifiedName: string;
  reportInstanceId: number;
  viewModel: ViewModel = new ViewModel();

  ngOnInit(): void {
    const self = this;

    this.itemForm = this._formBuilder.group({
      sourceIdentifier: [''],
    })

    self.workFlowId = self._activatedRoute.snapshot.paramMap.get('wuid');
    self.reportInstanceId = +self._activatedRoute.snapshot.paramMap.get('id');
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupBasic(null, null, null);
    self.loadData();
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.reportInstanceService.getReportInstanceExpanded(self.workFlowId, self.reportInstanceId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.qualifiedName = result.qualifiedName;
        self.updateForm(self.itemForm, result);
        self.viewModel.mainGrid.updateBasic(result.events);
      }, error => {
        this.handleError(error, "Error fetching report instance");
      });
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(ActivityHistoryComponent.name);
  }   

  setupTable() {
    if (this.currentScreenWidth === 'xs') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['activity', 'execution-event']);
    }
    if (this.currentScreenWidth === 'sm') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['activity', 'execution-event', 'executed-date']);
    }
  }

  downloadAttachment(activityExecutionStatusEventId: number, attachmentId: number): void {
    let self = this;
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.reportInstanceService.downloadAttachment(self.workFlowId, self.reportInstanceId, activityExecutionStatusEventId, attachmentId).subscribe(
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

            this.notify("Attachment downloaded successfully!", "Success");            
            break;
        }
      },
      error => {
        this.downloadStatus( {status: ProgressStatusEnum.ERROR} );
      }
    );
  }

  downloadStatus(event: ProgressStatus) {
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

  navigateToReportSearch(): void {
    let self = this;
    self._router.navigate([_routes.analytical.reports.searchByQualifiedName(self.workFlowId, self.qualifiedName)]);
  }   
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['activity', 'execution-event', 'executed-date',
              'comments', 'receipt-date', 'receipt-code', 'actions']);
}

class GridRecordModel {
  id: number;
  patientClinicalEventId: number;
  adverseEvent: string;
  activity: string;
  executionEvent: string;
  executedBy: string; 
  executedDate: string;
  comments: string; 
  receiptDate: string;
  receiptCode: string;
  patientSummaryFileId: number;
  patientExtractFileId: number;
  e2bXmlFileId: number;
}
