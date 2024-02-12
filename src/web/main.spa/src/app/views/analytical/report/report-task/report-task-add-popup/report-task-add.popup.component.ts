import { Component, OnInit, Inject } from '@angular/core';
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
  selector: 'report-task-add-popup',
  templateUrl: './report-task-add.popup.component.html'
})
export class ReportTaskAddPopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  public displayHelp: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ReportTaskAddPopupData,
    public dialogRef: MatDialogRef<ReportTaskAddPopupComponent>,
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
      source: ['', Validators.required],
      description: ['', [Validators.required, Validators.maxLength(500), Validators.pattern('[-a-zA-Z0-9()?,. ]*')]],
      taskType: ['', Validators.required],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.reportInstanceService.addTaskToReportInstanceCommand(this.data.workFlowGuid, self.data.reportInstanceId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Task successfully saved!", "Success");
        this.dialogRef.close(this.itemForm.value);
      }, error => {
        self.handleError(error, "Error saving task");
    });
  }
}

export interface ReportTaskAddPopupData {
  workFlowGuid: string;
  reportInstanceId: number;
  title: string;
}