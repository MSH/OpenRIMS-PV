import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { DatasetService } from 'app/shared/services/dataset.service';

@Component({
  templateUrl: './dataset.popup.component.html',
  animations: egretAnimations
})
export class DatasetPopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DatasetPopupData,
    public dialogRef: MatDialogRef<DatasetPopupComponent>,
    protected datasetService: DatasetService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      datasetName: [this.data.payload.datasetName || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z ]*')]],
      help: [this.data.payload.help || '', [Validators.maxLength(250), Validators.pattern('[a-zA-Z0-9. ]*')]]
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
    self.setBusy(true);

    self.datasetService.saveDataset(self.data.datasetId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset saved successfully", "Datasets");
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

export interface DatasetPopupData {
  datasetId: number;
  title: string;
  payload: any;
}