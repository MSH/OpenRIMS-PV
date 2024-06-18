import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Moment } from 'moment';
import * as moment from 'moment';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { PatientClinicalEventForUpdateModel } from 'app/shared/models/patient/patient-clinical-event-for-update.model';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';

@Component({
  templateUrl: './form-dynamic-adverse.popup.component.html',
  animations: egretAnimations
})
export class FormDynamicAdversePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  maxDate: Moment;

  customAttributeList: CustomAttributeDetailModel[] = [];
    
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<FormDynamicAdversePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected customAttributeService: CustomAttributeService,
    protected dialog: MatDialog
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.maxDate = moment();    
 
    self.viewModelForm = this._formBuilder.group({
      sourceDescription: ['', Validators.required],
      onsetDate: [ '', Validators.required ],
      resolutionDate: [ '' ],
      gravity: [null],
      serious: [null],
      severity: [null],
      treatment: [null],
      treatmentDetail: [''],
      outcome: [null],
    })

    self.getCustomAttributeList();
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.existingClinicalEvent != null) {
      self.loadData();
    }    
  }

  addOrUpdateClinicalEvent() {
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
      index: self.data.index,
      patientIdentifier: '',
      onsetDate,
      resolutionDate,
      sourceDescription: self.viewModelForm.get('sourceDescription').value,
      sourceTerminologyMedDraId: null,
      attributes: this.prepareAttributeForUpdateModel()
    };

    self.dialogRef.close(clinicalEventForUpdate);
  }

  private loadData(): void {
    let self = this;
    self.updateForm(self.viewModelForm, self.data.existingClinicalEvent);
  }

  private prepareAttributeForUpdateModel(): AttributeValueForPostModel[] {
    const attributesForUpdates: AttributeValueForPostModel[] = [];

    // gravity
    let customAttribute = this.customAttributeList.find(ca => ca.attributeKey == 'Intensity (Severity)')
    if(customAttribute != null) {
      const attributeForUpdateModel: AttributeValueForPostModel = {
        id: customAttribute.id,
        value: this.viewModelForm.get('gravity').value
      }
      attributesForUpdates.push(attributeForUpdateModel);
    }

    // is serious
    customAttribute = this.customAttributeList.find(ca => ca.attributeKey == 'Is the adverse event serious?')
    if(customAttribute != null) {
      const attributeForUpdateModel: AttributeValueForPostModel = {
        id: customAttribute.id,
        value: this.viewModelForm.get('serious').value
      }
      attributesForUpdates.push(attributeForUpdateModel);
    }

    // seriousness
    customAttribute = this.customAttributeList.find(ca => ca.attributeKey == 'Seriousness')
    if(customAttribute != null) {
      const attributeForUpdateModel: AttributeValueForPostModel = {
        id: customAttribute.id,
        value: this.viewModelForm.get('severity').value
      }
      attributesForUpdates.push(attributeForUpdateModel);
    }

    return attributesForUpdates;
  }

  private getCustomAttributeList(): void {
    let self = this;

    self.customAttributeService.getAllCustomAttributes('PatientClinicalEvent')
        .subscribe(result => {
            self.customAttributeList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }  
}

export interface PopupData {
  title: string;
  clinicalEventId: number;
  index: number;
  existingClinicalEvent: PatientClinicalEventForUpdateModel;
}