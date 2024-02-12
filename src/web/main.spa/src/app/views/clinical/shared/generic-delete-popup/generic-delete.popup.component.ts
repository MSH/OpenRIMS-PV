import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { finalize } from 'rxjs/operators';
import { AppointmentService } from 'app/shared/services/appointment.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { PatientService } from 'app/shared/services/patient.service';

@Component({
  templateUrl: './generic-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class GenericDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: GenericPopupData,
    public dialogRef: MatDialogRef<GenericDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected appointmentService: AppointmentService,
    protected patientService: PatientService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      name: [this.data.name],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

  }
}

export interface GenericPopupData {
  patientId: number;
  id: number;
  title: string;
  type: '';
  name: string;
}