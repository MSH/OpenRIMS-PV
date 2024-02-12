import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Router } from '@angular/router';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { DatasetService } from 'app/shared/services/dataset.service';

@Component({
  templateUrl: './dataset-delete.popup.component.html',
  animations: egretAnimations
})
export class DatasetDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<DatasetDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected datasetService: DatasetService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      datasetName: [this.data.payload.datasetName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.datasetService.deleteDataset(self.data.datasetId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset deleted successfully", "Datasets");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error deleting dataset");
    });
  }
}

export interface PopupData {
  datasetId: number;
  title: string;
  payload: any;
}