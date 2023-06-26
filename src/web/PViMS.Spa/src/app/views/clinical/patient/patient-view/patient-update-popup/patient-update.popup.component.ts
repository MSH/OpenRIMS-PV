import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { concatMap, finalize, tap } from 'rxjs/operators';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { PatientCustomAttributesForUpdateModel } from 'app/shared/models/patient/patient-custom-attributes-for-update.model';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientNameForUpdateModel } from 'app/shared/models/patient/patient-name-for-update.model';
import { PatientDateOfBirthForUpdateModel } from 'app/shared/models/patient/patient-date-of-birth-for-update.model';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { PatientFacilityForUpdateModel } from 'app/shared/models/patient/patient-facility-for-update.model';
import { PatientNotesForUpdateModel } from 'app/shared/models/patient/patient-notes-for-update.model';
const moment =  _moment;

@Component({
  templateUrl: './patient-update.popup.component.html',
  animations: egretAnimations
})
export class PatientUpdatePopupComponent extends BasePopupComponent  implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  facilityList: string[] = [];
  customAttributeList: CustomAttributeDetailModel[] = [];
  patientAttributes: AttributeValueModel[];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PatientUpdatePopupData,
    public dialogRef: MatDialogRef<PatientUpdatePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService
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
      facilityName: [null, Validators.required],
      attributes: this._formBuilder.group([]),
      notes: [''],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.patientId > 0) {
      self.loadData();
    }
  }  
  
  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientDetail(self.data.patientId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);
        self.patientAttributes = result.patientAttributes;

        self.getCustomAttributeList();
        self.markFormGroupTouched(self.viewModelForm);
      }, error => {
        self.throwError(error, error.statusText);
      });
  }   

  submit(): void {
    let self = this;
    self.setBusy(true);

    var patientCustomAttributesForUpdate = self.preparePatientCustomAttributesForUpdateModel();
    var patientDateOfBirthForUpdate = self.preparePatientDateOfBirthForUpdateModel();
    var patientFacilityForUpdate = self.preparePatientFacilityForUpdateModel();
    var patientNameForUpdate = self.preparePatientNameForUpdateModel();
    var patientNotesForUpdate = self.preparePatientNotesForUpdateModel();

    this.patientService.updatePatientCustomAttributes(self.data.patientId, patientCustomAttributesForUpdate)
      .pipe(
        concatMap((res: any) => this.patientService.updatePatientDateOfBirth(self.data.patientId, patientDateOfBirthForUpdate)),
        concatMap((res: any) => this.patientService.updatePatientFacility(self.data.patientId, patientFacilityForUpdate)),
        concatMap((res: any) => this.patientService.updatePatientName(self.data.patientId, patientNameForUpdate)),
        concatMap((res: any) => this.patientService.updatePatientNotes(self.data.patientId, patientNotesForUpdate))
      )
      .subscribe(
        data => {
          self.setBusy(false);
          self.notify("Patient successfully updated!", "Success");
          this.dialogRef.close(this.viewModelForm.value);
        },
        error => {
          this.handleError(error, "Error updating patient");
        });
  }

  private preparePatientCustomAttributesForUpdateModel(): PatientCustomAttributesForUpdateModel {
    const self = this;
    const patientCustomAttributesForUpdate: PatientCustomAttributesForUpdateModel = 
    {
      attributes: self.prepareAttributeForUpdateModel()
    };

    return patientCustomAttributesForUpdate;
  }

  private preparePatientDateOfBirthForUpdateModel(): PatientDateOfBirthForUpdateModel {
    const self = this;

    let dateOfBirth = self.viewModelForm.get('dateOfBirth').value;
    if(moment.isMoment(self.viewModelForm.get('dateOfBirth').value)) {
      dateOfBirth = self.viewModelForm.get('dateOfBirth').value.format('YYYY-MM-DD');
    }

    const patientDateOfBirthForUpdate: PatientDateOfBirthForUpdateModel = 
    {
      dateOfBirth
    };

    return patientDateOfBirthForUpdate;
  }

  private preparePatientFacilityForUpdateModel(): PatientFacilityForUpdateModel {
    const self = this;
    const patientFacilityForUpdate: PatientFacilityForUpdateModel = 
    {
      facilityName: self.viewModelForm.get('facilityName').value
    };

    return patientFacilityForUpdate;
  }

  private preparePatientNameForUpdateModel(): PatientNameForUpdateModel {
    const self = this;
    const patientNameForUpdate: PatientNameForUpdateModel = 
    {
      firstName: self.viewModelForm.get('firstName').value,
      middleName: self.viewModelForm.get('middleName').value,
      lastName: self.viewModelForm.get('lastName').value
    };

    return patientNameForUpdate;
  }

  private preparePatientNotesForUpdateModel(): PatientNotesForUpdateModel {
    const self = this;
    const patientNotesForUpdate: PatientNotesForUpdateModel = 
    {
      notes: self.viewModelForm.get('notes').value
    };

    return patientNotesForUpdate;
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

  private loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
  }  

  private getFacilityList(): void {
    let self = this;
    self.facilityList = self.accountService.facilities;
  }

  private getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;

    self.customAttributeService.getAllCustomAttributes('Patient')
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
              let patientAttribute = self.patientAttributes.find(pa => pa.key == attribute.attributeKey);
              attributes.addControl(attribute.id.toString(), new FormControl(patientAttribute != null ? patientAttribute.value : defaultValue, validators));
            })

        }, error => {
          this.handleError(error, "Error loading patient attributes");
        });
  }
}

export interface PatientUpdatePopupData {
  patientId: number;
  title: string;
}