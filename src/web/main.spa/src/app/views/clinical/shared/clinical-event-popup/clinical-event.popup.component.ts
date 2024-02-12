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
import { MeddraSelectPopupComponent } from 'app/shared/components/popup/meddra-select-popup/meddra-select.popup.component';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientClinicalEventForUpdateModel } from 'app/shared/models/patient/patient-clinical-event-for-update.model';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
const moment =  _moment;

@Component({
  templateUrl: './clinical-event.popup.component.html',
  animations: egretAnimations
})
export class ClinicalEventPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  clinicalEventAttributes: AttributeValueModel[];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ClinicalEventPopupData,
    public dialogRef: MatDialogRef<ClinicalEventPopupComponent>,
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
      sourceDescription: ['', [Validators.required, Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., '/\n/\r/\t/\s]*")]],
      sourceTerminologyMedDraId: [''],
      medDraTerm: [''],
      onsetDate: ['', Validators.required],
      resolutionDate: [''],
      attributes: this._formBuilder.group([])
    })

    if(self.data.clinicalEventId == 0) {
      self.getCustomAttributeList();
    }
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.clinicalEventId > 0) {
      self.loadData();
    }
  }  
  
  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientClinicalEventDetail(self.data.patientId, self.data.clinicalEventId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);
        self.clinicalEventAttributes = result.clinicalEventAttributes;

        self.CLogFormErrors(self.viewModelForm);
        self.getCustomAttributeList();
        self.markFormGroupTouched(self.viewModelForm);        
      }, error => {
        self.handleError(error, "Error fetching patient clinical event");
      });
  }   

  openMeddraPopup() {
    let self = this;
    let title = 'Select Meddra Term';
    let dialogRef: MatDialogRef<any> = self.dialog.open(MeddraSelectPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        self.updateForm(self.viewModelForm, {sourceTerminologyMedDraId: result.id});
        self.updateForm(self.viewModelForm, {medDraTerm: result.medDraTerm});
      })
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    let clinicalEventForUpdate = this.prepareClinicalEventForUpdateModel();
    self.patientService.savePatientClinicalEvent(self.data.patientId, self.data.clinicalEventId, clinicalEventForUpdate)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Clinical event successfully updated!", "Success");
        this.dialogRef.close(this.viewModelForm.value);
    }, error => {
      this.handleError(error, "Error saving clinical event");
    });      
  }

  private getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;
    self.customAttributeService.getAllCustomAttributes('PatientClinicalEvent')
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
              if(self.data.clinicalEventId > 0) {
                let clinicalEventAttribute = self.clinicalEventAttributes.find(pa => pa.key == attribute.attributeKey);
                attributes.addControl(attribute.id.toString(), new FormControl(clinicalEventAttribute != null ? clinicalEventAttribute.value : defaultValue, validators));
              }
              else {
                attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));                
              }
            })

        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  private prepareClinicalEventForUpdateModel(): PatientClinicalEventForUpdateModel {
    let self = this;
    self.setBusy(true);

    let onsetDate = self.viewModelForm.get('onsetDate').value;
    if(moment.isMoment(self.viewModelForm.get('onsetDate').value)) {
      onsetDate = self.viewModelForm.get('onsetDate').value.format('YYYY-MM-DD');
    }
    let resolutionDate = '';
    if(moment.isMoment(self.viewModelForm.get('resolutionDate').value)) {
      resolutionDate = self.viewModelForm.get('resolutionDate').value.format('YYYY-MM-DD');
    }
    else {
      if (self.viewModelForm.get('resolutionDate').value != '') {
        resolutionDate = self.viewModelForm.get('resolutionDate').value;
      }
    }

    const clinicalEventForUpdate: PatientClinicalEventForUpdateModel = 
    {
      id: self.data.clinicalEventId,
      index: 0,
      patientIdentifier: '',
      onsetDate,
      resolutionDate,
      sourceDescription: self.viewModelForm.get('sourceDescription').value,
      sourceTerminologyMedDraId: self.viewModelForm.get('sourceTerminologyMedDraId').value != null ? +self.viewModelForm.get('sourceTerminologyMedDraId').value : null,
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

export interface ClinicalEventPopupData {
  patientId: number;
  clinicalEventId: number;
  title: string;
}