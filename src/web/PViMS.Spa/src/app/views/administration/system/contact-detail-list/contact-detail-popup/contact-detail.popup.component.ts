import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ContactDetailService } from 'app/shared/services/contact-detail.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './contact-detail.popup.component.html',
  animations: egretAnimations
})
export class ContactDetailPopupComponent extends BasePopupComponent  implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ContactDetailPopupData,
    public dialogRef: MatDialogRef<ContactDetailPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected contactDetailService: ContactDetailService,
    protected popupService: PopupService,
    protected accountService: AccountService
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      contactType: [this.data.payload.contactType || ''],
      organisationType: ['', [Validators.required]],
      organisationName: [this.data.payload.organisationName || '', [Validators.required, Validators.maxLength(60), Validators.pattern('[a-zA-Z0-9 ]*')]],
      departmentName: [this.data.payload.departmentName || '', [Validators.required, Validators.maxLength(60), Validators.pattern('[a-zA-Z0-9 ]*')]],
      contactFirstName: [this.data.payload.contactFirstName || '', [Validators.required, Validators.maxLength(35), Validators.pattern('[a-zA-Z ]*')]],
      contactLastName: [this.data.payload.contactLastName || '', [Validators.required, Validators.maxLength(35), Validators.pattern('[a-zA-Z ]*')]],
      streetAddress: [this.data.payload.streetAddress || '', [Validators.required, Validators.maxLength(100), Validators.pattern('[a-zA-Z0-9 ]*')]],
      city: [this.data.payload.city || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z ]*')]],
      state: [this.data.payload.state || '', [Validators.maxLength(50), Validators.pattern('[a-zA-Z ]*')]],
      countryCode: [this.data.payload.countryCode || '', [Validators.maxLength(10), Validators.pattern('[0-9]*')]],
      postCode: [this.data.payload.postCode || '', [Validators.maxLength(20), Validators.pattern('[a-zA-Z0-9]*')]],
      contactNumber: [this.data.payload.contactNumber || '', [Validators.maxLength(50), Validators.pattern('[-0-9]*')]],
      contactEmail: [this.data.payload.contactEmail || '', [Validators.maxLength(50), Validators.pattern('[-a-zA-Z0-9@._]*')]]
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.contactId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.contactDetailService.getContactDetail(self.data.contactId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.CLog(result);
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        self.handleError(error, "Error fetching contact details");
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.contactDetailService.saveContact(self.data.contactId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Contact saved successfully", "Contact Details");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        self.handleError(error, "Error saving contact details");
    });
  }
}

export interface ContactDetailPopupData {
  contactId: number;
  title: string;
  payload: any;
}