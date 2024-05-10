import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { LabTestIdentifierModel } from 'app/shared/models/labs/lab-test.identifier.model';
import { LabTestService } from 'app/shared/services/lab-test.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { PatientLabTestForUpdateModel } from 'app/shared/models/patient/patient-lab-test-for-update.model';
import { Moment } from 'moment';
// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { forkJoin } from 'rxjs';
import { LabResultIdentifierModel } from 'app/shared/models/labs/lab-result.identifier.model';
import { LabTestUnitIdentifierModel } from 'app/shared/models/labs/lab-test-unit.identifier.model';
const moment =  _moment;

@Component({
  templateUrl: './form-dynamic-labs.popup.component.html',
  animations: egretAnimations
})
export class FormDynamicLabsPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  viewModel: ViewModel = new ViewModel();

  maxDate: Moment;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<FormDynamicLabsPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected labTestService: LabTestService,
    protected dialog: MatDialog
  ) { 
    super(_router, _location, popupService, accountService);
  } 

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.maxDate = moment();

    self.viewModelForm = this._formBuilder.group({
      labTest: ['', Validators.required],
      testDate: ['', Validators.required],
      testResultCoded: [''],
      testResultValue: ['', [Validators.maxLength(20), Validators.pattern("[-a-zA-Z0-9 .]*")]],
      testUnit: [''],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.existingLabTest != null) {
      self.loadData();
    }    
  }

  addOrUpdateLabTest() {
    let self = this;
    self.setBusy(true);

    let testDate = self.viewModelForm.get('testDate').value;
    if(moment.isMoment(self.viewModelForm.get('testDate').value)) {
      testDate = self.viewModelForm.get('testDate').value.format('YYYY-MM-DD');
    }

    const labTestForUpdate: PatientLabTestForUpdateModel = 
    {
      id: self.data.labTestId,
      index: self.data.index,
      labTest: self.viewModelForm.get('labTest').value,
      testDate,
      testResultCoded: self.viewModelForm.get('testResultCoded').value,
      testResultValue: self.viewModelForm.get('testResultValue').value,
      testUnit: self.viewModelForm.get('testUnit').value,
      referenceLower: '',
      referenceUpper: '',
      attributes: []
    };

    self.dialogRef.close(labTestForUpdate);
  }

  private loadData(): void {
    let self = this;
    self.updateForm(self.viewModelForm, self.data.existingLabTest);
  }   

  private loadDropDowns(): void {
    let self = this;

    const requestArray = [];

    requestArray.push(self.labTestService.getAllLabTests());
    requestArray.push(self.labTestService.getAllLabResults());
    requestArray.push(self.labTestService.getAllLabTestUnits());

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.viewModel.labTestList =  data[0] as LabTestIdentifierModel[];
          self.viewModel.labResultList =  data[1] as LabResultIdentifierModel[];
          self.viewModel.labTestUnitList =  data[2] as LabTestUnitIdentifierModel[];

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });    
  } 
}

class ViewModel {
  labTestList: LabTestIdentifierModel[] = [];
  labResultList: LabResultIdentifierModel[] = [];
  labTestUnitList: LabTestUnitIdentifierModel[] = [];  
}

export interface PopupData {
  title: string;
  labTestId: number;
  index: number;
  existingLabTest: PatientLabTestForUpdateModel;
}