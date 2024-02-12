import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { FacilityService } from 'app/shared/services/facility.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { FacilityTypeIdentifierModel, FacilityTypeIdentifierWrapperModel } from 'app/shared/models/facility/facility-type.identifier.model';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { forkJoin } from 'rxjs';
import { OrgUnitService } from 'app/shared/services/org-unit.service';
import { OrgUnitIdentifierModel } from 'app/shared/models/org-unit/org-unit.identifier.model';

@Component({
  templateUrl: './facility.popup.component.html',
  animations: egretAnimations
})
export class FacilityPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  typeList: FacilityTypeIdentifierModel[] = [];
  orgUnitList: OrgUnitIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FacilityPopupData,
    public dialogRef: MatDialogRef<FacilityPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected facilityService: FacilityService,
    protected orgUnitService: OrgUnitService,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected formBuilder: FormBuilder,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this.formBuilder.group({
      facilityName: [this.data.payload.facilityName || '', [Validators.required, Validators.maxLength(100), Validators.pattern("[-a-zA-Z0-9. '()]*")]],
      facilityCode: [this.data.payload.facilityCode || '', [Validators.required, Validators.maxLength(18), Validators.pattern("[-a-zA-Z0-9]*")]],
      facilityType: [this.data.payload.facilityType || '', Validators.required],
      contactNumber: [this.data.payload.contactNumber || '', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z0-9]*")]],
      mobileNumber: [this.data.payload.mobileNumber || '', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z0-9]*")]],
      faxNumber: [this.data.payload.faxNumber || '', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z0-9]*")]],
      orgUnitId: [this.data.payload.orgUnitId || ''],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.facilityId > 0) {
        self.loadData();
    }
  }
  
  submit() {
    let self = this;
    self.setBusy(true);

    self.facilityService.saveFacility(self.data.facilityId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Facility successfully saved!", "Success");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error saving facility");        
    });
  }

  private loadDropDowns(): void {
    let self = this;

    const requestArray = [];

    requestArray.push(self.facilityService.getFacilityTypeList());
    requestArray.push(self.orgUnitService.getAllOrgUnits());

    forkJoin(requestArray)
      .subscribe(
        data => {
          let facilityTypes = data[0] as FacilityTypeIdentifierWrapperModel;
          self.typeList = facilityTypes.value;

          self.orgUnitList = data[1] as OrgUnitIdentifierModel[];

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });    
  }

  private loadData(): void {
    let self = this;
    self.setBusy(true);
    self.facilityService.getFacilityDetail(self.data.facilityId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        self.handleError(error, 'error fetching facility detail');
      });
  }   
}

export interface FacilityPopupData {
  facilityId: number;
  title: string;
  payload: any;
}