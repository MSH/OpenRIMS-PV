import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { EnrolmentService } from 'app/shared/services/enrolment.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './enrolment.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class EnrolmentPopupComponent extends BasePopupComponent  implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EnrolmentPopupData,
    public dialogRef: MatDialogRef<EnrolmentPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected enrolmentService: EnrolmentService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
        cohort: [this.data.cohort || ''],
        conditionStartDate: [this.data.conditionStartDate || ''],
        enrolmentDate: [this.data.payload.enrolmentDate || '', Validators.required],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.enrolmentService.saveEnrolment(this.data.patientId, this.data.cohortGroupId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Enrolment successfully saved!", "Success");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      if(error.status == 400) {
        self.showInfo(error.message[0], error.statusText);
      } else {
        if(Array.isArray(error.message)) {
          self.showInfo(error.message[0], error.statusText);
        }
        else {
          self.throwError(error, error.statusText);
        }
      }
  });
  }
}

export interface EnrolmentPopupData {
    patientId: number;
    cohortGroupId: number;
    cohort: string;
    conditionStartDate: string;
    title: string;
    payload: any;
}