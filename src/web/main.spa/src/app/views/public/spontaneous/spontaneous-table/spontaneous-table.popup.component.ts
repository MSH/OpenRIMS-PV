import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { DatasetElementSubViewModel } from 'app/shared/models/dataset/dataset-element-sub-view.model';

@Component({
  templateUrl: './spontaneous-table.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class SpontaneousTablePopupComponent extends BasePopupComponent  implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: SpontaneousTablePopupData,
    public dialogRef: MatDialogRef<SpontaneousTablePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      elements: this._formBuilder.group([]),
    })
    // Add sub elements to form group
    let elements = self.itemForm.get('elements') as FormGroup;
    self.data.datasetElementSubs.forEach(element => {
      let validators = [ ];
      if(element.required) {
          validators.push(Validators.required);
      }
      if(element.stringMaxLength != null) {
          validators.push(Validators.maxLength(element.stringMaxLength));
      }
      if(element.numericMinValue != null && element.numericMaxValue != null) {
          validators.push(Validators.max(element.numericMaxValue));
          validators.push(Validators.min(element.numericMinValue));
      }

      let value = null;
      if (self.data.payload) {
        value = self.data.payload[element.datasetElementSubId.toString()];
      }

      elements.addControl(element.datasetElementSubId.toString(), new FormControl(value, validators));
    })
  }

  ngAfterViewInit(): void {
    let self = this;
  }    

  submit() {
    let self = this;
    self.notify("Record saved successfully", "Successfull");
    self.dialogRef.close(this.itemForm.value);
  }
}

export interface SpontaneousTablePopupData {
  title: string;
  datasetElementSubs: DatasetElementSubViewModel[];
  payload: any;
}