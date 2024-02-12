import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { LabTestService } from 'app/shared/services/lab-test.service';

@Component({
  templateUrl: './form-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormDeletePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: LabTestPopupData,
    public dialogRef: MatDialogRef<FormDeletePopupComponent>,
    protected labTestService: LabTestService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      labResultName: [this.data.payload.labResultName],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.labResultId > 0) {
        self.loadData();
    }
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

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.labTestService.getLabResultIdentifier(self.data.labResultId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        self.throwError(error, error.statusText);
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.labTestService.deleteLabResult(self.data.labResultId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Test result deleted successfully", "Tests Results");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        if(error.status == 400) {
          self.showInfo(error.error.message[0], error.statusText);
        } else {
          self.throwError(error, error.statusText);
        }
    });
  }
}

export interface LabTestPopupData {
  labResultId: number;
  title: string;
  payload: any;
}