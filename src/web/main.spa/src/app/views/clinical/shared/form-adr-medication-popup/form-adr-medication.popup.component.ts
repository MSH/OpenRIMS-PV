import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { ConceptSelectPopupComponent } from 'app/shared/components/popup/concept-select-popup/concept-select.popup.component';
import { PatientMedicationForUpdateModel } from 'app/shared/models/patient/patient-medication-for-update.model';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
const moment =  _moment;

@Component({
  templateUrl: './form-adr-medication.popup.component.html',
  animations: egretAnimations
})
export class FormADRMedicationPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  medicationAttributes: AttributeValueModel[];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FormADRMedicationPopupData,
    public dialogRef: MatDialogRef<FormADRMedicationPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService,
    protected dialog: MatDialog
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      conceptId: ['', Validators.required],
      productId: [''],
      medication: [''],
      startDate: ['', Validators.required],
      endDate: [''],
      dose: ['', [Validators.maxLength(30), Validators.pattern("[a-zA-Z0-9./]*")]],
      doseUnit: [null],
      doseFrequency: ['', [Validators.maxLength(30), Validators.pattern("[a-zA-Z0-9. ]*")]],
      attributes: this._formBuilder.group([])
    })

    if(self.data.existingMedication == null) {
      self.getCustomAttributeList();
    }
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.existingMedication != null) {
      self.loadData();
    }    
  }
  
  loadData(): void {
    let self = this;
    self.updateForm(self.viewModelForm, self.data.existingMedication);
    self.getCustomAttributeList();
  }  

  getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;
    self.customAttributeService.getAllCustomAttributes('PatientMedication')
        .subscribe(result => {
            self.customAttributeList = result;

            // Add custom attributes to form group
            self.customAttributeList.forEach(attribute => {
              var defaultValue = '';
              if(attribute.customAttributeType == 'Selection') {
                defaultValue = null;
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
              if(self.data.existingMedication != null) {
                let medicationAttribute = self.data.existingMedication.attributes.find(ma => ma.id == attribute.id);
                attributes.addControl(attribute.id.toString(), new FormControl(medicationAttribute == null ? defaultValue : medicationAttribute.value, validators));
              }
              else {
                attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));                
              }              
            })

        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  openConceptPopup() {
    let self = this;
    let title = 'Select Medication';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ConceptSelectPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, productOnly: false }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        if(result.source == "Concept") {
          self.updateForm(self.viewModelForm, {conceptId: result.id});
          self.updateForm(self.viewModelForm, {productId: 0});
          self.updateForm(self.viewModelForm, {medication: result.displayName});
        }
        else {
          self.updateForm(self.viewModelForm, {conceptId: 0});
          self.updateForm(self.viewModelForm, {productId: result.id});
          self.updateForm(self.viewModelForm, {medication: result.displayName});
        }
      })
  }

  addOrUpdateMedication() {
    let self = this;
    self.setBusy(true);

    let startDate = self.viewModelForm.get('startDate').value;
    if(moment.isMoment(self.viewModelForm.get('startDate').value)) {
      startDate = self.viewModelForm.get('startDate').value.format('YYYY-MM-DD');
    }
    let endDate = '';
    if(moment.isMoment(self.viewModelForm.get('endDate').value)) {
      endDate = self.viewModelForm.get('endDate').value.format('YYYY-MM-DD');
    }
    else {
      if (self.viewModelForm.get('endDate').value != '') {
        endDate = self.viewModelForm.get('endDate').value;
      }
    }
    let productId = 0;
    if(self.viewModelForm.get('productId').value) {
      productId = +self.viewModelForm.get('productId').value
    }

    const medicationModel: PatientMedicationForUpdateModel = {
      id: self.data.medicationId,
      medication: self.viewModelForm.get('medication').value,
      index: self.data.index,
      sourceDescription: '',
      conceptId: +self.viewModelForm.get('conceptId').value,
      productId: productId,
      startDate: startDate,
      endDate: endDate,
      dose: self.viewModelForm.get('dose').value,
      doseFrequency: self.viewModelForm.get('doseFrequency').value,
      doseUnit: self.viewModelForm.get('doseUnit').value,
      attributes: this.prepareAttributeForUpdateModel()
    };

    self.dialogRef.close(medicationModel);    
  }

  private prepareAttributeForUpdateModel(): AttributeValueForPostModel[] {
    const attributesForPosts: AttributeValueForPostModel[] = [];
    this.customAttributeList.forEach(element => {
      const attributeForUpdateModel: AttributeValueForPostModel = {
        id: element.id,
        value: this.viewModelForm.get('attributes').value[element.id]
      }
      attributesForPosts.push(attributeForUpdateModel);
    });
    return attributesForPosts;
  }
}

export interface FormADRMedicationPopupData {
  title: string;
  medicationId: number;
  index: number;
  existingMedication: PatientMedicationForUpdateModel;
}