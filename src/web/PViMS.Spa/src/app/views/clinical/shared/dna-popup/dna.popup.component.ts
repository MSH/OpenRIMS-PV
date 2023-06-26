import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { AppointmentService } from 'app/shared/services/appointment.service';

@Component({
  templateUrl: './dna.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class DnaPopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DnaPopupData,
    public dialogRef: MatDialogRef<DnaPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected appointmentService: AppointmentService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
        appointmentDate: [this.data.payload.appointmentDate || '']
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.appointmentService.markAppointmentAsDNA(this.data.payload.patientId, this.data.payload.id)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Appointment marked as DNA!", "Success");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        self.handleError(error, "Error marking appointment as DNA");
    });
  }
}

export interface DnaPopupData {
  title: string;
  payload: any;
}