import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
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
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ReportTaskAddPopupComponent } from './report-task-add-popup/report-task-add.popup.component';
import { ChangeTaskDetailsPopupComponent } from './change-task-details-popup/change-task-details.popup.component';
import { _routes } from 'app/config/routes';
import { TaskCommentsPopupComponent } from 'app/shared/components/popup/task-comments-popup/task-comments.popup.component';
import { ChangeTaskStatusPopupComponent } from 'app/shared/components/popup/change-task-status-popup/change-task-status.popup.component';

@Component({
  templateUrl: './report-task-list.component.html',
  styles: [`
    .mat-column-source { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-description { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-task-type { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-task-status { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-created { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-updated { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-comment-count { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],
  animations: egretAnimations
})
export class ReportTaskListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected reportInstanceService: ReportInstanceService,
    protected eventService: EventService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog) 
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
        self.viewModel.mainGrid.updateBasic(result.tasks);
      }, error => {
        this.handleError(error, "Error fetching report instance");
      });
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(ReportTaskListComponent.name);
  }   

  setupTable() {
    if (this.currentScreenWidth === 'xs') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['source', 'description']);
    }
    if (this.currentScreenWidth === 'sm') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['source', 'description', 'task-type']);
    }
  }

  openTaskAddPopUp() {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(ReportTaskAddPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { workFlowGuid: this.workFlowId, title: 'Add Task', reportInstanceId: this.reportInstanceId }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openChangeTaskDetailsPopUp(reportInstanceTaskId: number) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(ChangeTaskDetailsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { workFlowGuid: this.workFlowId, title: 'Change Task Details', reportInstanceId: this.reportInstanceId, reportInstanceTaskId }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }  

  openChangeTaskStatusPopUp(reportInstanceTaskId: number) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(ChangeTaskStatusPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { workFlowGuid: this.workFlowId, title: 'Change Task Status', reportInstanceId: this.reportInstanceId, reportInstanceTaskId }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openTaskCommentsPopUp(reportInstanceTaskId: number) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(TaskCommentsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { workFlowGuid: this.workFlowId, title: 'Comments', reportInstanceId: this.reportInstanceId, reportInstanceTaskId }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        self.loadData();
      })
  }

  navigateToReportSearch(): void {
    let self = this;
    self._router.navigate([_routes.analytical.reports.searchByQualifiedName(self.workFlowId, self.qualifiedName)]);
  }  
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['source', 'description', 'task-type',
              'task-status', 'created', 'updated', 'comment-count', 'actions']);
}

class GridRecordModel {
  id: number;
  source: string;
  description: string;
  taskType: string;
  taskStatus: string;
  createdDetail: string;
  updatedDetail: string;
}
