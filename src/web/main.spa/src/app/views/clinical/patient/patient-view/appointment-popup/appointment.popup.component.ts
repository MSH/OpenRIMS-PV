import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { AppointmentService } from 'app/shared/services/appointment.service';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'appointment-popup',
  templateUrl: './appointment.popup.component.html'
})
export class AppointmentPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  currentDate = new Date();
  minDate = new Date();
  maxDate = new Date(this.currentDate.getFullYear() + 2, this.currentDate.getMonth(), this.currentDate.getDate());

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AppointmentPopupData,
    public dialogRef: MatDialogRef<AppointmentPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected appointmentService: AppointmentService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
        appointmentDate: [this.data.payload.appointmentDate || '', Validators.required],
        reason: [this.data.payload.reason || '', [Validators.required, Validators.maxLength(250), Validators.pattern('[a-zA-Z0-9 ]*')]],
        cancelled: [this.data.payload.cancelled || ''],
        cancellationReason: [this.data.payload.cancellationReason, [Validators.maxLength(250), Validators.pattern('[a-zA-Z0-9 ]*')]],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.appointmentId > 0) {
        self.loadData();
    }
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.appointmentService.getAppointmentDetail(self.data.patientId, self.data.appointmentId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (this.data.payload = result));
      }, error => {
        self.handleError(error, "Error loading appointment");
      });
  } 

  submit() {
    let self = this;
    self.setBusy(true);

    self.appointmentService.saveAppointment(this.data.patientId, self.data.appointmentId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Appointment successfully saved!", "Success");
        this.dialogRef.close(this.itemForm.value);
      }, error => {
        self.handleError(error, "Error saving appointment");
    });
  }
}

export interface AppointmentPopupData {
  patientId: number;
  appointmentId: number;
  title: string;
  payload: any;
}