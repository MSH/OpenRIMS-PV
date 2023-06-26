import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './custom-attribute-edit.popup.component.html',
  animations: egretAnimations
})
export class CustomAttributeEditPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  viewModel: ViewModel = new ViewModel();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<CustomAttributeEditPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected customAttributeService: CustomAttributeService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      extendableTypeName: [this.data.extendableTypeName],
      customAttributeType: ['', [Validators.required]],
      category: ['', [Validators.required, Validators.maxLength(100), Validators.pattern('[a-zA-Z() ]*')]],
      attributeKey: ['', [Validators.required, Validators.maxLength(100), Validators.pattern('[a-zA-Z ]*')]],
      attributeDetail: ['', [Validators.maxLength(150), Validators.pattern('[-a-zA-Z0-9 ]*')]],
      isRequired: ['No', Validators.required],
      isSearchable: ['No', Validators.required],
      stringMaxLength: [''],
      numericMinValue: [''],
      numericMaxValue: [''],
      futureDateOnly: ['No'],
      pastDateOnly: ['No'],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.customAttributeId > 0) {
        self.loadData();
    }
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.customAttributeService.getCustomAttributeDetail(self.data.customAttributeId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, result);
      }, error => {
        self.handleError(error, "Error fetching attribute");
      });
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.customAttributeService.saveCustomAttribute(self.data.customAttributeId, self.itemForm.value)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Custom attribute saved successfully", "Attributes");
      this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error saving attribute");
   });
  }
}

export interface PopupData {
  customAttributeId: number;
  extendableTypeName: string;
  title: string;
}

class ViewModel {
  customAttribute: CustomAttributeDetailModel;
}