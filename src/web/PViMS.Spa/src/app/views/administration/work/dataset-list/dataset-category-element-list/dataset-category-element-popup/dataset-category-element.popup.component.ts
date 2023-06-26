import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { DatasetService } from 'app/shared/services/dataset.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './dataset-category-element.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class DatasetCategoryElementPopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DatasetCategoryElementPopupData,
    public dialogRef: MatDialogRef<DatasetCategoryElementPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected datasetService: DatasetService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = self._formBuilder.group({
      elementName: [self.data.payload.elementName],
      friendlyName: [self.data.payload.friendlyName || '', [Validators.maxLength(150), Validators.pattern('[a-zA-Z0-9. ]*')]],
      help: [self.data.payload.help || '', [Validators.maxLength(350), Validators.pattern('[a-zA-Z0-9. ]*')]],
      acute: [self.data.payload.acute, Validators.required],
      chronic: [self.data.payload.chronic, Validators.required]
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.datasetService.saveDatasetCategoryElement(self.data.datasetId, self.data.datasetCategoryId, self.data.datasetCategoryElementId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset category element saved successfully", "Datasets");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        self.handleError(error, "Error saving dataset category element");
    });
  }
}

export interface DatasetCategoryElementPopupData {
  datasetId: number;
  datasetCategoryId: number;
  datasetCategoryElementId: number;
  title: string;
  payload: any;
}