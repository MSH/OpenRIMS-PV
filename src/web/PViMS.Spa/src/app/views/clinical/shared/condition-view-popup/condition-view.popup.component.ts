import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
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
import { ConditionService } from 'app/shared/services/condition.service';

@Component({
  templateUrl: './condition-view.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class ConditionViewPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  conditionAttributes: AttributeValueModel[];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ConditionViewPopupData,
    public dialogRef: MatDialogRef<ConditionViewPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected conditionService: ConditionService,
    protected customAttributeService: CustomAttributeService,
    protected dialog: MatDialog
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      sourceDescription: [''],
      medDraTerm: [''],
      startDate: [''],
      outcomeDate: [''],
      outcome: [''],
      treatmentOutcome: [''],
      comments: [''],
      attributes: this._formBuilder.group([])
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.conditionId > 0) {
      self.loadData();
    }
  }  
  
  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientConditionDetail(self.data.patientId, self.data.conditionId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);
        self.conditionAttributes = result.conditionAttributes;

        self.getCustomAttributeList();
      }, error => {
        this.handleError(error, "Error loading condition");
      });
  }   

  getCustomAttributeList(): void {
    let self = this;

    let attributes = self.viewModelForm.get('attributes') as FormGroup;
    self.customAttributeService.getAllCustomAttributes('PatientCondition')
        .subscribe(result => {
            self.customAttributeList = result;

            // Add custom attributes to form group
            self.customAttributeList.forEach(attribute => {
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
              if(self.data.conditionId > 0) {
                let conditionAttribute = self.conditionAttributes.find(pa => pa.key == attribute.attributeKey);
                let value = '';
                if (conditionAttribute != null) {
                  value = conditionAttribute.value;
                  if (conditionAttribute.selectionValue != '') {
                    value = conditionAttribute.selectionValue;
                  }
                }
                attributes.addControl(attribute.id.toString(), new FormControl(value, validators));
              }
              else {
                attributes.addControl(attribute.id.toString(), new FormControl('', validators));                
              }
            })

        }, error => {
          this.handleError(error, "Error loading condition attributes");
        });
  }
}

export interface ConditionViewPopupData {
  patientId: number;
  conditionId: number;
  title: string;
}