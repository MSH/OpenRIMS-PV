import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { DatePipe, Location } from '@angular/common';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { PatientConditionForUpdateModel } from 'app/shared/models/patient/patient-condition-for-update.model';
const moment =  _moment;

@Component({
  templateUrl: './form-dynamic-conditions.popup.component.html',
  animations: egretAnimations
})
export class FormDynamicConditionsPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  viewModel: ViewModel = new ViewModel();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<FormDynamicConditionsPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected dialog: MatDialog,
    protected datePipe: DatePipe
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = this._formBuilder.group({
      condition: [null, Validators.required],
      conditionStatus: [null, Validators.required]
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.existingCondition != null) {
      self.loadData();
    }    
  }   

  addOrUpdateCondition() {
    let self = this;
    self.setBusy(true);

    let currentDate = new Date();
    let onsetDate = this.datePipe.transform(currentDate, 'yyyy-MM-dd');

    const conditionModel: PatientConditionForUpdateModel = {
      id: self.data.conditionId,
      condition: self.viewModelForm.get('condition').value,
      index: self.data.index,
      sourceDescription: self.viewModelForm.get('condition').value,
      sourceTerminologyMedDraId: 0,
      onsetDate: onsetDate,
      outcome: '',
      treatmentOutcome: '',
      caseNumber: '',
      comments: '',
      attributes: []
    };

    self.dialogRef.close(conditionModel);
  }

  private loadData(): void {
    let self = this;
    self.updateForm(self.viewModelForm, self.data.existingCondition);
  }   
}

class ViewModel {
}

export interface PopupData {
  title: string;
  conditionId: number;
  index: number;
  existingCondition: PatientConditionForUpdateModel;
}