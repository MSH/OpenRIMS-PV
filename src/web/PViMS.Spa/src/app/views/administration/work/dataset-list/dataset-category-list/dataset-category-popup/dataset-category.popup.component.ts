import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { DatasetService } from 'app/shared/services/dataset.service';

@Component({
  templateUrl: './dataset-category.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class DatasetCategoryPopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DatasetCategoryPopupData,
    public dialogRef: MatDialogRef<DatasetCategoryPopupComponent>,
    protected datasetService: DatasetService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;

    self.itemForm = self.formBuilder.group({
      datasetCategoryName: [self.data.payload.datasetCategoryName || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z ]*')]],
      friendlyName: [self.data.payload.friendlyName || '', [Validators.maxLength(150), Validators.pattern('[a-zA-Z0-9. ]*')]],
      help: [self.data.payload.help || '', [Validators.maxLength(350), Validators.pattern('[a-zA-Z0-9. ]*')]],
      acute: [self.data.payload.acute, Validators.required],
      chronic: [self.data.payload.chronic, Validators.required]
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

    self.datasetService.saveDatasetCategory(self.data.datasetId, self.data.datasetCategoryId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset category saved successfully", "Datasets");
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

export interface DatasetCategoryPopupData {
  datasetId: number;
  datasetCategoryId: number;
  title: string;
  payload: any;
}