import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MeddraTermService } from 'app/shared/services/meddra-term.service';
import { finalize } from 'rxjs/operators';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';

@Component({
  templateUrl: './set-classification.popup.component.html',
  animations: egretAnimations
})
export class SetClassificationPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  public itemForm: FormGroup;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: SetClassificationPopupData,
    public dialogRef: MatDialogRef<SetClassificationPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected meddraTermService: MeddraTermService,
    protected reportInstanceService: ReportInstanceService
  ) { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
      reportClassification: ['', Validators.required],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    self.loadData();
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.reportInstanceService.getReportInstanceDetail(self.data.workFlowId, self.data.reportInstanceId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, result);
      }, error => {
        this.handleError(error, "Error fetching report instance");
      });
  }

  setClassification() {
    let self = this;
    self.setBusy(true);

    self.reportInstanceService.updateReportInstanceClassification(self.data.workFlowId, self.data.reportInstanceId, self.itemForm.value)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Classification set successfully", "Success");
      self.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error updating classification");
    });
  }  
}

export interface SetClassificationPopupData {
  workFlowId: string;
  reportInstanceId: number;
  title: string;
}