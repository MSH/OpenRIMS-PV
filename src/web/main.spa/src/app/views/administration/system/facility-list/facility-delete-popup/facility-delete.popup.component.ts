import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { FacilityService } from 'app/shared/services/facility.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './facility-delete.popup.component.html',
  animations: egretAnimations
})
export class FacilityDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FacilityPopupData,
    public dialogRef: MatDialogRef<FacilityDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected facilityService: FacilityService,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected formBuilder: FormBuilder,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      facilityName: [this.data.payload.facilityName],
      facilityCode: [this.data.payload.facilityCode],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.facilityService.deleteFacility(self.data.facilityId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Facility deleted successfully", "Facilities");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        self.handleError(error, 'error deleting facility');
    });
  }
}

export interface FacilityPopupData {
  facilityId: number;
  title: string;
  payload: any;
}