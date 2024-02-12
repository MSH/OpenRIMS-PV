import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { EnrolmentService } from 'app/shared/services/enrolment.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  selector: 'deenrolment-popup',
  templateUrl: './deenrolment.popup.component.html'
})
export class DeenrolmentPopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EnrolmentPopupData,
    public dialogRef: MatDialogRef<DeenrolmentPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected enrolmentService: EnrolmentService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
        cohort: [this.data.cohort || ''],
        enroledDate: [this.data.enroledDate || ''],
        deenroledDate: [this.data.deenroledDate || '', Validators.required],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.enrolmentService.saveDeenrolment(this.data.patientId, this.data.cohortGroupEnrolmentId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("De-enrolment successfully saved!", "Success");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        if(error.status == 400) {
          self.showInfo(error.error.message, error.statusText);
        } else {
          self.throwError(error, error.statusText);
        }
    });
  }
}

export interface EnrolmentPopupData {
    patientId: number;
    cohortGroupEnrolmentId: number;
    cohort: string;
    enroledDate: string;
    deenroledDate: Date;
    title: string;
    payload: any;
}