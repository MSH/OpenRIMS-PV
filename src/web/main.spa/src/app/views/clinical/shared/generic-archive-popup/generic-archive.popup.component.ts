import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { finalize } from 'rxjs/operators';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { PatientService } from 'app/shared/services/patient.service';
import { AppointmentService } from 'app/shared/services/appointment.service';
import { EnrolmentService } from 'app/shared/services/enrolment.service';

@Component({
  templateUrl: './generic-archive.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class GenericArchivePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: GenericArchivePopupData,
    public dialogRef: MatDialogRef<GenericArchivePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected appointmentService: AppointmentService,
    protected enrolmentService: EnrolmentService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      name: [this.data.name],
      reason: ['', [Validators.required, Validators.maxLength(200), Validators.pattern("[a-zA-Z0-9 '.]*")]]
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    switch (self.data.type) {
      case "Patient Condition":
        self.patientService.archivePatientCondition(self.data.parentId, self.data.id, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error archiving " + self.data.type);
        });
  
        break;

      case "Patient Clinical Event":
        self.patientService.archivePatientClinicalEvent(self.data.parentId, self.data.id, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error archiving " + self.data.type);
        });
  
        break;

      case "Patient Medication":
        self.patientService.archivePatientMedication(self.data.parentId, self.data.id, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error archiving " + self.data.type);
        });
    
        break;

      case "Patient Lab Test":
        self.patientService.archivePatientLabTest(self.data.parentId, self.data.id, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error archiving " + self.data.type);
        });
    
        break;

      case "Appointment":
        self.appointmentService.archiveAppointment(self.data.parentId, self.data.id, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error archiving " + self.data.type);
        });
    
        break;

      case "Attachment":
        self.patientService.archiveAttachment(self.data.parentId, self.data.id, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error archiving " + self.data.type);
        });
    
        break;

      case "Enrolment":
        self.enrolmentService.archiveEnrolment(self.data.parentId, self.data.id, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error archiving " + self.data.type);
        });
    
        break;

      case "Patient":
        self.patientService.archivePatient(self.data.id, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error archiving " + self.data.type);
        });
    
        break;
    }    
  }
}

export interface GenericArchivePopupData {
  parentId: number;
  id: number;
  title: string;
  type: 'Patient Condition' | 'Patient Clinical Event' | 'Patient Medication' | 'Patient Lab Test' | 'Appointment' | 'Attachment' | 'Enrolment' | 'Patient';
  payload: any;
  name: string;
}