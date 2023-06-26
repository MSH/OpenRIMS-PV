import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import * as moment from 'moment';

@Component({
  templateUrl: './form-b-adverse.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormBAdversePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FormBAdversePopupData,
    public dialogRef: MatDialogRef<FormBAdversePopupComponent>,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;
 
    self.itemForm = this.formBuilder.group({
      adverseEvent: [this.data.payload.adverseEvent || '', Validators.required],
      adverseEventStartDate: [ Object.keys(this.data.payload).length > 0 ? this.data.payload.startDate.length > 0 ? moment(this.data.payload.startDate, "YYYY-MM-DD") : '' : '', Validators.required ],
      adverseEventEndDate: [ Object.keys(this.data.payload).length > 0 ? this.data.payload.endDate.length > 0 ? moment(this.data.payload.endDate, "YYYY-MM-DD") : '' : '' ],
      gravity: [this.data.payload.gravity || ''],
      serious: [this.data.payload.serious || ''],
      severity: [this.data.payload.severity || '']
    })
  }

  public setBusy(value: boolean): void {
    setTimeout(() => { this.busy = value; });
  }

  public isBusy(): boolean {
    return this.busy;
  }

  protected notify(message: string, action: string) {
    return this.popupService.notify(message, action);
  }

  protected showError(errorMessage: any, title: string = "Error") {
    this.popupService.showErrorMessage(errorMessage, title);
  }

  protected showInfo(message: string, title: string = "Info") {
    this.popupService.showInfoMessage(message, title);
  }

  protected updateForm(form: FormGroup, value: any): void {
    form.patchValue(value);
  }  

  protected throwError(errorObject: any, title: string = "Exception") {
    if (errorObject.status == 401) {
        this.showError(errorObject.error.message, errorObject.error.statusCodeType);
    } else {
        this.showError(errorObject.message, title);
    }
  }

  submit() {
    let self = this;
    self.notify("Adverse event saved successfully", "Adverse Events");
    this.dialogRef.close(this.itemForm.value);
  }
}

export interface FormBAdversePopupData {
  title: string;
  payload: any;
}