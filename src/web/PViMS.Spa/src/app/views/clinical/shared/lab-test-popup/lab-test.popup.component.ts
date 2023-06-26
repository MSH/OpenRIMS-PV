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
import { finalize } from 'rxjs/operators';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { LabTestIdentifierModel } from 'app/shared/models/labs/lab-test.identifier.model';
import { LabResultIdentifierModel } from 'app/shared/models/labs/lab-result.identifier.model';
import { LabTestUnitIdentifierModel } from 'app/shared/models/labs/lab-test-unit.identifier.model';
import { LabTestService } from 'app/shared/services/lab-test.service';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientLabTestForUpdateModel } from 'app/shared/models/patient/patient-lab-test-for-update.model';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { forkJoin } from 'rxjs';
const moment =  _moment;

@Component({
  templateUrl: './lab-test.popup.component.html',
  animations: egretAnimations
})
export class LabTestPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  labTestAttributes: AttributeValueModel[];
  labTestList: LabTestIdentifierModel[] = [];
  labResultList: LabResultIdentifierModel[] = [];
  labTestUnitList: LabTestUnitIdentifierModel[] = [];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: LabTestPopupData,
    public dialogRef: MatDialogRef<LabTestPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected labTestService: LabTestService,
    protected customAttributeService: CustomAttributeService,
    protected dialog: MatDialog
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      labTest: ['', Validators.required],
      testDate: ['', Validators.required],
      testResultCoded: [''],
      testResultValue: ['', [Validators.maxLength(20), Validators.pattern("[-a-zA-Z0-9 .]*")]],
      testUnit: [''],
      referenceLower: ['', [Validators.maxLength(20), Validators.pattern("[-a-zA-Z0-9 .]*")]],
      referenceUpper: ['', [Validators.maxLength(20), Validators.pattern("[-a-zA-Z0-9 .]*")]],
      attributes: this._formBuilder.group([])
    })

    if(self.data.labTestId == 0) {
      self.getCustomAttributeList();
    }
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.labTestId > 0) {
      self.loadData();
    }
  }  
  
  submit() {
    let self = this;
    self.setBusy(true);
    
    let labTestForUpdate = this.prepareLabTestForUpdateModel();
    self.patientService.savePatientLabTest(self.data.patientId, self.data.labTestId, labTestForUpdate)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Lab test successfully updated!", "Success");
        this.dialogRef.close(this.viewModelForm.value);
    }, error => {
      this.handleError(error, "Error saving lab test");
    });      
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
          self.labTestList =  data[0] as LabTestIdentifierModel[];
          self.labResultList =  data[1] as LabResultIdentifierModel[];
          self.labTestUnitList =  data[2] as LabTestUnitIdentifierModel[];

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });    
  }  

  private loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientLabTestDetail(self.data.patientId, self.data.labTestId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);
        self.labTestAttributes = result.labTestAttributes;

        self.getCustomAttributeList();
        self.markFormGroupTouched(self.viewModelForm);        
      }, error => {
        this.handleError(error, "Error fetching patient lab test");
      });
  }   

  private getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;
    self.customAttributeService.getAllCustomAttributes('PatientLabTest')
        .subscribe(result => {
            self.customAttributeList = result;

            // Add custom attributes to form group
            self.customAttributeList.forEach(attribute => {
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
              if(self.data.labTestId > 0) {
                let labTestAttribute = self.labTestAttributes.find(pa => pa.key == attribute.attributeKey);
                attributes.addControl(attribute.id.toString(), new FormControl(labTestAttribute != null ? labTestAttribute.value : defaultValue, validators));
              }
              else {
                attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));                
              }
            })

        }, error => {
          this.handleError(error, "Error fetching patient lab test attributes");
        });
  }

  private prepareLabTestForUpdateModel(): PatientLabTestForUpdateModel {
    let self = this;
    self.setBusy(true);

    let testDate = self.viewModelForm.get('testDate').value;
    if(moment.isMoment(self.viewModelForm.get('testDate').value)) {
      testDate = self.viewModelForm.get('testDate').value.format('YYYY-MM-DD');
    }

    const clinicalEventForUpdate: PatientLabTestForUpdateModel = 
    {
      id: self.data.labTestId,
      index: 0,
      labTest: self.viewModelForm.get('labTest').value,
      testDate,
      testResultCoded: self.viewModelForm.get('testResultCoded').value,
      testResultValue: self.viewModelForm.get('testResultValue').value,
      testUnit: self.viewModelForm.get('testUnit').value,
      referenceLower: self.viewModelForm.get('referenceLower').value,
      referenceUpper: self.viewModelForm.get('referenceUpper').value,
      attributes: this.prepareAttributeForUpdateModel()
    };

    return clinicalEventForUpdate;
  }

  private prepareAttributeForUpdateModel(): AttributeValueForPostModel[] {
    const attributesForUpdates: AttributeValueForPostModel[] = [];
    this.customAttributeList.forEach(element => {
      const attributeForUpdateModel: AttributeValueForPostModel = {
        id: element.id,
        value: this.viewModelForm.get('attributes').value[element.id]
      }
      attributesForUpdates.push(attributeForUpdateModel);
    });
    return attributesForUpdates;
  }  
}

export interface LabTestPopupData {
  patientId: number;
  labTestId: number;
  title: string;
}