import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { MetaFormService } from 'app/shared/services/meta-form.service';

@Component({
  templateUrl: './form-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FormDeletePopupData,
    public dialogRef: MatDialogRef<FormDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected metaFormService: MetaFormService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
    })

  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.metaFormService.deleteForm(self.data.id).then(response =>
      {
        if (response) {
            self.notify('Form deleted successfully!', 'Form Deleted');
            self.dialogRef.close(true)
        }
        else {
            self.showError('There was an error deleting form, please try again !', 'Delete');
        }
      });   
  }  

}

export interface FormDeletePopupData {
  id: number;
  title: string;
}