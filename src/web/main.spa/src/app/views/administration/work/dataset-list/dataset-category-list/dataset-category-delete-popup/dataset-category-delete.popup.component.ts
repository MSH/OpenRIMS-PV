import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
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
  templateUrl: './dataset-category-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class DatasetCategoryDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DatasetCategoryPopupData,
    public dialogRef: MatDialogRef<DatasetCategoryDeletePopupComponent>,
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
      categoryName: [this.data.payload.datasetCategoryName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.datasetService.deleteDatasetCategory(self.data.datasetId, self.data.datasetCategoryId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset category deleted successfully", "Dataset Categories");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error deleting dataset category");
    });
  }
}

export interface DatasetCategoryPopupData {
  datasetId: number;
  datasetCategoryId: number;
  title: string;
  payload: any;
}