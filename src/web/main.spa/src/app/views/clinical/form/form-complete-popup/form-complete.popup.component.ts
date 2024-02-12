import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { Form } from 'app/shared/indexed-db/appdb';

@Component({
  templateUrl: './form-complete.popup.component.html',
  animations: egretAnimations
})
export class FormCompletePopupComponent extends BasePopupComponent implements OnInit {
  
  viewModel: ViewModel = new ViewModel();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FormCompletePopupData,
    public dialogRef: MatDialogRef<FormCompletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected accountService: AccountService,
    protected metaFormService: MetaFormService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) 
  { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;
    self.loadFormData();
  }

  loadFormData(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getForm(self.data.formId).then(result => {
        let form = result as Form;
        self.viewModel.formIdentifier = form.formIdentifier;
        self.setBusy(false);
    }, error => {
          self.throwError(error, error.statusText);
    });
  }   

  confirmFormComplete(): void {
    const self = this;
    self.metaFormService.markFormAsCompleted(self.data.formId).then(response =>
      {
        if (response) {
          self.notify('Form marked as completed!', 'Form Saved');
          self.viewModel.confirmed = true;
        }
        else {
          self.showError('There was an error updating the form locally, please try again !', 'Form Error');
        }
      });
  }
}

export interface FormCompletePopupData {
  formId: number;
  title: string;
}

class ViewModel {
  confirmed = false;
  formIdentifier: string;
}