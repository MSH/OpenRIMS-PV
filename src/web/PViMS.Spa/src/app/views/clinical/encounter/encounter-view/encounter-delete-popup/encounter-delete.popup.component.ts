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
import { EncounterService } from 'app/shared/services/encounter.service';

@Component({
  templateUrl: './encounter-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class EncounterDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EncounterDeletePopupData,
    public dialogRef: MatDialogRef<EncounterDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected encounterService: EncounterService
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      encounterDate: [this.data.payload.encounterDate],
      encounterType: [this.data.payload.encounterType],
      reason: ['', [Validators.required, Validators.maxLength(200), Validators.pattern("[a-zA-Z0-9 '.]*")]]
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.encounterService.archiveEncounter(self.data.patientId, self.data.encounterId, self.itemForm.value)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Encounter deleted successfully", "Encounters");
      this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error archiving encounter");      
    });
  }
}

export interface EncounterDeletePopupData {
  patientId: number;
  encounterId: number;
  title: string;
  payload: any;
}