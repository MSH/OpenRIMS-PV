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
import { ConditionService } from 'app/shared/services/condition.service';
import { OutcomeIdentifierModel } from 'app/shared/models/condition/outcome.identifier.model';
import { TreatmentOutcomeIdentifierModel } from 'app/shared/models/condition/treatment-outcome.identifier.model';
import { MeddraSelectPopupComponent } from 'app/shared/components/popup/meddra-select-popup/meddra-select.popup.component';

@Component({
  templateUrl: './condition.popup.component.html',
  animations: egretAnimations
})
export class ConditionPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  conditionAttributes: AttributeValueModel[];
  outcomeList: OutcomeIdentifierModel[] = [];
  treatmentOutcomeList: TreatmentOutcomeIdentifierModel[] = [];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ConditionPopupData,
    public dialogRef: MatDialogRef<ConditionPopupComponent>,
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
    self.loadDropDowns();

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      sourceDescription: ['', [Validators.required, Validators.maxLength(200), Validators.pattern("[-a-zA-Z0-9 .,()']*")]],
      sourceTerminologyMedDraId: ['', Validators.required],
      medDraTerm: ['', Validators.required],
      startDate: ['', Validators.required],
      outcomeDate: [''],
      outcome: [''],
      treatmentOutcome: [''],
      caseNumber: ['', [Validators.required, Validators.maxLength(50), Validators.pattern("[-a-zA-Z0-9 .()]*")]],
      comments: ['', [Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9 ']*")]],
      attributes: this._formBuilder.group([])
    })

    if(self.data.conditionId == 0) {
      self.getCustomAttributeList();
    }
  }

  loadDropDowns(): void {
    let self = this;
    self.getOutcomeList();
    self.getTreatmentOutcomeList();
  }

  getOutcomeList(): void {
    let self = this;

    self.conditionService.getAllOutcomes()
        .subscribe(result => {
          self.outcomeList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  getTreatmentOutcomeList(): void {
    let self = this;

    self.conditionService.getAllTreatmentOutcomes()
        .subscribe(result => {
          self.treatmentOutcomeList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
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
        self.markFormGroupTouched(self.viewModelForm);
      }, error => {
        self.throwError(error, error.statusText);
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
              if(self.data.conditionId > 0) {
                let conditionAttribute = self.conditionAttributes.find(pa => pa.key == attribute.attributeKey);
                attributes.addControl(attribute.id.toString(), new FormControl(conditionAttribute != null ? conditionAttribute.value : defaultValue, validators));
              }
              else {
                attributes.addControl(attribute.id.toString(), new FormControl(defaultValue, validators));                
              }
            })

        }, error => {
            self.throwError(error, error.statusText);
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
    self.patientService.savePatientCondition(self.data.patientId, self.data.conditionId, self.viewModelForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Condition successfully updated!", "Success");
        this.dialogRef.close(this.viewModelForm.value);
    }, error => {
      this.handleError(error, "Error saving condition");
    });      
  }
}

export interface ConditionPopupData {
  patientId: number;
  conditionId: number;
  title: string;
}