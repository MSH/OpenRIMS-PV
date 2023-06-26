import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { finalize } from 'rxjs/operators';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './activity-status-change.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class ActivityStatusChangePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ActivityPopupData,
    public dialogRef: MatDialogRef<ActivityStatusChangePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected reportInstanceService: ReportInstanceService,
    protected popupService: PopupService,
    protected accountService: AccountService
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      currentStatus: [self.data.currentStatus],
      newExecutionStatus: [self.data.newStatus],
      comments: ['', [Validators.maxLength(100), Validators.pattern('[a-zA-Z0-9 .,()]*')]],
      contextDate: [''],
      contextCode: ['', [Validators.maxLength(20), Validators.pattern('[-a-zA-Z0-9]*')]],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.reportInstanceService.updateReportInstanceActivity(self.data.workFlowId, self.data.reportInstanceId, self.itemForm.value)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Status changed successfully", "Activity");
      this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error updating status");
    });
  }
}

export interface ActivityPopupData {
  workFlowId: string;
  reportInstanceId: number;
  currentStatus: string;
  newStatus: string;
  title: string;
}