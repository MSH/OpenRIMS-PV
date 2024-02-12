import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { finalize } from 'rxjs/operators';
import { ConditionService } from 'app/shared/services/condition.service';
import { EncounterTypeService } from 'app/shared/services/encounter-type.service';
import { EncounterTypeIdentifierModel } from 'app/shared/models/encounter/encounter-type.identifier.model';
import { PriorityIdentifierModel } from 'app/shared/models/encounter/priority.identifier.model';
import { PriorityService } from 'app/shared/services/priority.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { ConditionDetailModel } from 'app/shared/models/condition/condition.detail.model';
import { forkJoin } from 'rxjs';
import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientForCreationModel } from 'app/shared/models/patient/patient-for-creation.model';
const moment =  _moment;

@Component({
  templateUrl: './patient-add.popup.component.html',
  animations: egretAnimations
})
export class PatientAddPopupComponent extends BasePopupComponent implements OnInit {
  
  public viewModelForm: FormGroup;

  viewModel: ViewModel = new ViewModel();

  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PatientAddPopupData,
    public dialogRef: MatDialogRef<PatientAddPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService,
    protected conditionService: ConditionService,
    protected encounterTypeService: EncounterTypeService,
    protected priorityService: PriorityService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      firstName: ['', [Validators.required, Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      lastName: ['', [Validators.required, Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      middleName: ['', [Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      dateOfBirth: ['', Validators.required],
      facilityName: ['', Validators.required],
      attributes: this._formBuilder.group([]),
      conditionGroupId: ['', Validators.required],
      meddraTermId: ['', Validators.required],
      cohortGroupId: ['', Validators.required],
      enroledDate: ['', Validators.required],
      startDate: ['', Validators.required],
      outcomeDate: [''],
      caseNumber: ['', [Validators.required, Validators.maxLength(50), Validators.pattern("[-a-zA-Z0-9 .()]*")]],
      comments: ['', [Validators.maxLength(100), Validators.pattern("[-a-zA-Z0-9 .,()']*")]],
      encounterTypeId: [1, Validators.required],
      priorityId: [1, Validators.required],
      encounterDate: [moment(), Validators.required],
    })

    self.getCustomAttributeList();
  }

  onConditionSelected(event) {
    const value = event.value;
    var selections = this.viewModel.conditionList.filter(c => c.id == value);
    this.viewModel.selectedCondition = this.viewModel.conditionList.filter(c => c.id == value)[0];
  }

  submit() {
    let self = this;
    self.setBusy(true);

    if(self.data.patientId == 0) {
      self.patientService.savePatient(self.preparePatientForCreationModel())
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify("Patient successfully saved!", "Success");
          this.dialogRef.close(this.viewModelForm.value);
      }, error => {
        this.handleError(error, "Error saving patient");
      });
    }
  }

  private preparePatientForCreationModel(): PatientForCreationModel {
    let self = this;
    let outcomeDate = '';
    if(moment.isMoment(self.viewModelForm.get('outcomeDate').value)) {
      outcomeDate = self.viewModelForm.get('outcomeDate').value.format('YYYY-MM-DD');
    }
    else {
      if (self.viewModelForm.get('outcomeDate').value != '') {
        outcomeDate = self.viewModelForm.get('outcomeDate').value;
      }
    }

    const attributesForUpdate: AttributeValueForPostModel[] = [];
    self.viewModel.customAttributeList.forEach(attribute => {
      attributesForUpdate.push(self.prepareAttributeValue(attribute.attributeKey, attribute.id.toString()));
    })

    const patientForCreationModel: PatientForCreationModel = 
    {
      firstName: self.viewModelForm.get('firstName').value,
      lastName: self.viewModelForm.get('lastName').value,
      middleName: self.viewModelForm.get('middleName').value,
      dateOfBirth: self.viewModelForm.get('dateOfBirth').value.format('YYYY-MM-DD'),
      facilityName: self.viewModelForm.get('facilityName').value,
      conditionGroupId: +self.viewModelForm.get('conditionGroupId').value,
      meddraTermId: +self.viewModelForm.get('meddraTermId').value,
      cohortGroupId: +self.viewModelForm.get('cohortGroupId').value,
      enroledDate: self.viewModelForm.get('enroledDate').value.format('YYYY-MM-DD'),
      startDate: self.viewModelForm.get('startDate').value.format('YYYY-MM-DD'),
      outcomeDate: outcomeDate,
      caseNumber: self.viewModelForm.get('caseNumber').value,
      comments: self.viewModelForm.get('comments').value,
      encounterTypeId: +self.viewModelForm.get('encounterTypeId').value,
      priorityId: +self.viewModelForm.get('priorityId').value,
      encounterDate: self.viewModelForm.get('encounterDate').value.format('YYYY-MM-DD'),
      attributes: attributesForUpdate
    };

    return patientForCreationModel;
  }

  private prepareAttributeValue(attributeKey: string, formKey: string): AttributeValueForPostModel {
    const self = this;
    let customAttribute = self.viewModel.customAttributeList.find(ca => ca.attributeKey.toLowerCase() == attributeKey.toLowerCase());
    if(customAttribute == null) {
      return null;
    }

    let attributes = self.viewModelForm.get('attributes') as FormGroup;
    const attributeForPost: AttributeValueForPostModel = {
      id: customAttribute.id,
      value: attributes.value[formKey]
    }
    self.CLog(attributeForPost, 'attributeForPost');
    return attributeForPost;
  }  

  private loadDropDowns(): void {
    const self = this;
    self.setBusy(true);

    const requestArray = [];
    
    requestArray.push(self.conditionService.getAllConditions());
    requestArray.push(self.encounterTypeService.getAllEncounterTypes());
    requestArray.push(self.priorityService.getAllPriorities());

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.viewModel.conditionList = data[0] as ConditionDetailModel[];
          self.viewModel.encounterTypeList = data[1] as EncounterTypeIdentifierModel[];
          self.viewModel.priorityList = data[2] as PriorityIdentifierModel[];

          self.viewModel.facilityList = self.accountService.facilities;

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });

  }  

  private getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;

    self.customAttributeService.getAllCustomAttributes('Patient')
        .subscribe(result => {
            self.viewModel.customAttributeList = result;

            // Add custom attributes to form group
            self.viewModel.customAttributeList.forEach(attribute => {
              var defaultValue = '';
              if(attribute.customAttributeType == 'Selection') {
                defaultValue = '0';
              }

              let validators = [ ];
              if(attribute.required) {
                validators.push(Validators.required);
              }
              if(attribute.stringMaxLength != null) {
                validators.push(Validators.maxLength(attribute.stringMaxLength));
              }
              if(attribute.numericMinValue != null && attribute.numericMaxValue != null) {
                validators.push(Validators.max(attribute.numericMaxValue));
                validators.push(Validators.min(attribute.numericMinValue));
              }
              attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));
            })

        }, error => {
            self.throwError(error, error.statusText);
        });
  }
}

export interface PatientAddPopupData {
  patientId: number;
  title: string;
  payload: any;
}

class ViewModel {
  facilityList: string[] = [];
  customAttributeList: CustomAttributeDetailModel[] = [];
  conditionList: ConditionDetailModel[] = [];
  selectedCondition: ConditionDetailModel;
  encounterTypeList: EncounterTypeIdentifierModel[] = [];
  priorityList: PriorityIdentifierModel[] = [];
}