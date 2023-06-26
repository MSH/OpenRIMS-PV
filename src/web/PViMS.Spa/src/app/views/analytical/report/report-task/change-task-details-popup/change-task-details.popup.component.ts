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

@Component({
  selector: 'change-task-details-popup',
  templateUrl: './change-task-details.popup.component.html'
})
export class ChangeTaskDetailsPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ChangeTaskDetailsPopupData,
    public dialogRef: MatDialogRef<ChangeTaskDetailsPopupComponent>,
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
      source: ['', [Validators.required, Validators.maxLength(200), Validators.pattern('[a-zA-Z0-9 ]*')]],
      description: ['', [Validators.required, Validators.maxLength(500), Validators.pattern('[-a-zA-Z0-9()?,. ]*')]]
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
        self.updateForm(self.itemForm, result);
      }, error => {
        self.throwError(error, error.statusText);
      });
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.reportInstanceService.changeTaskDetailsCommand(this.data.workFlowGuid, self.data.reportInstanceId, self.data.reportInstanceTaskId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Task details successfully updated!", "Success");
        this.dialogRef.close(this.itemForm.value);
      }, error => {
        self.handleError(error, "Error saving task details");
    });
  }
}

export interface ChangeTaskDetailsPopupData {
  workFlowGuid: string;
  reportInstanceId: number;
  reportInstanceTaskId: number;
  title: string;
}