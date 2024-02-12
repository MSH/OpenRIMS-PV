import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { DatasetService } from 'app/shared/services/dataset.service';

@Component({
  templateUrl: './dataset-category-element-delete.popup.component.html',
  animations: egretAnimations
})
export class DatasetCategoryElementDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DatasetCategoryElementPopupData,
    public dialogRef: MatDialogRef<DatasetCategoryElementDeletePopupComponent>,
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

    self.itemForm = this._formBuilder.group({
      categoryName: [this.data.categoryName],
      elementName: [this.data.payload.elementName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);
    console.log(self.data.datasetCategoryElementId);
    self.datasetService.deleteDatasetCategoryElement(self.data.datasetId, self.data.datasetCategoryId, self.data.datasetCategoryElementId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset element deleted successfully", "Dataset Elements");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        self.handleError(error, "Error deleting dataset category element");
    });
  }
}

export interface DatasetCategoryElementPopupData {
  datasetId: number;
  datasetCategoryId: number;
  datasetCategoryElementId: number;
  categoryName: string;
  title: string;
  payload: any;
}