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
import { DatasetElementService } from 'app/shared/services/dataset-element.service';

@Component({
  templateUrl: './dataset-element-sub-delete.popup.component.html',
  animations: egretAnimations
})
export class DatasetElementSubDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<DatasetElementSubDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected datasetElementService: DatasetElementService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      elementName: [this.data.payload.elementName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.datasetElementService.deleteDatasetElementSub(self.data.datasetElementId, self.data.datasetElementSubId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset element sub deleted successfully", "Dataset Elements");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error deleting dataset element sub");            
    });
  }
}

export interface PopupData {
  datasetElementId: number;
  datasetElementSubId: number;
  title: string;
  payload: any;
}