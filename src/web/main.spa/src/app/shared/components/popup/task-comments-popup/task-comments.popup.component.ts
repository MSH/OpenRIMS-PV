import { Component, OnInit, Inject, AfterViewInit, ViewChild } from '@angular/core';
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
  selector: 'task-comments-popup',
  templateUrl: './task-comments.popup.component.html'
})
export class TaskCommentsPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  public task: TaskModel;
  
  @ViewChild('form') form;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: TaskCommentsPopupData,
    public dialogRef: MatDialogRef<TaskCommentsPopupComponent>,
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
      comment: ['', [Validators.required, Validators.maxLength(500)]],
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

  submit() {
    let self = this;
    self.setBusy(true);

    self.reportInstanceService.addCommentToReportInstanceTaskCommand(this.data.workFlowGuid, self.data.reportInstanceId, self.data.reportInstanceTaskId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Comment successfully saved!", "Success");
        self.loadData();
        self.itemForm.reset();
        self.form.resetForm();
      }, error => {
        self.handleError(error, "Error saving comment");
    });
  }
}

export interface TaskCommentsPopupData {
  workFlowGuid: string;
  reportInstanceId: number;
  reportInstanceTaskId: number;
  title: string;
}