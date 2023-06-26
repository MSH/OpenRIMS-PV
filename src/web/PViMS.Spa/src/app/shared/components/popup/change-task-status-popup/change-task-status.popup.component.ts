import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { TaskModel } from 'app/shared/models/report-instance/task.model';

@Component({
  selector: 'change-task-status-popup',
  templateUrl: './change-task-status.popup.component.html'
})
export class ChangeTaskStatusPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  public task: TaskModel;
  protected busy: boolean = false;


  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ChangeTaskStatusPopupData,
    public dialogRef: MatDialogRef<ChangeTaskStatusPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected reportInstanceService: ReportInstanceService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
      taskStatus: ['', Validators.required],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    self.loadData();
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.reportInstanceService.getReportInstanceTaskDetail(self.data.workFlowGuid, self.data.reportInstanceId, self.data.reportInstanceTaskId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.task = result;
        self.updateForm(self.itemForm, result);
      }, error => {
        self.throwError(error, error.statusText);
      });
  }



  changeTaskStatus(taskStatus: 'Acknowledged' | 'On Hold' | 'Completed' | 'Cancelled' | 'Done') {
    let self = this;
    self.setBusy(true);
    self.updateForm(self.itemForm, {taskStatus});
    self.reportInstanceService.changeTaskStatusCommand(this.data.workFlowGuid, self.data.reportInstanceId, self.data.reportInstanceTaskId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Task status successfully updated!", "Success");
        this.dialogRef.close(this.itemForm.value);
      }, error => {
        self.handleError(error, "Error saving task status");
    });
  }

  allowStatusAcknowledge(): boolean {
    if(this.accountService.hasRole('Clinician') || this.accountService.hasRole('DataCap')) {
      if(this.task?.taskStatus == 'New')  {
        return true
      }
    }
    return false;
  }

  allowStatusDone(): boolean {
    if(this.accountService.hasRole('Clinician') || this.accountService.hasRole('DataCap')) {
      if(this.task?.taskStatus == 'Acknowledged')  {
        return true
      }
    }
    return false;
  }  

  allowStatusOnHold(): boolean {
    if(this.accountService.hasRole('Analyst')) {
      return true
    }
    return false;
  }

  allowStatusCancelled(): boolean {
    if(this.accountService.hasRole('Analyst')) {
      return true
    }
    return false;
  }

  allowStatusComplete(): boolean {
    if(this.accountService.hasRole('Analyst')) {
      if(this.task?.taskStatus == 'Done')  {
        return true
      }
    }
    return false;
  }
}

export interface ChangeTaskStatusPopupData {
  workFlowGuid: string;
  reportInstanceId: number;
  reportInstanceTaskId: number;
  title: string;
}