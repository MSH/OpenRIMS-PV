import { Component, OnInit, AfterViewInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, RequiredValidator, ValidatorFn } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription, Observable } from 'rxjs';
import { FacilityService } from 'app/shared/services/facility.service';
import { takeUntil, finalize, startWith, map } from 'rxjs/operators';
import { Moment } from 'moment';
// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { GridModel } from 'app/shared/models/grid.model';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { _routes } from 'app/config/routes';
import { Form } from 'app/shared/indexed-db/appdb';
import { FacilityIdentifierModel } from 'app/shared/models/facility/facility.identifier.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormAConditionsPopupComponent } from './form-a-conditions-popup/form-a-conditions.popup.component';
import { FormALabsPopupComponent } from './form-a-labs-popup/form-a-labs.popup.component';
import { FormAMedicationsPopupComponent } from './form-a-medications-popup/form-a-medications.popup.component';
import { FormCompletePopupComponent } from '../form-complete-popup/form-complete.popup.component';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';
import { FormGuidelinesPopupComponent } from '../form-guidelines-popup/form-guidelines.popup.component';

const moment =  _moment;

@Component({
  selector: 'app-form-a',
  templateUrl: './form-a.component.html',
  styleUrls: ['./form-a.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormAComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected facilityService: FacilityService,
    protected dialog: MatDialog,
    protected metaFormService: MetaFormService,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          //this.setupTable();
      }
    });    
  }

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  isComplete = false;
  isSynched = false;

  id: number;
  identifier: string;
  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;
  viewPatientModelForm: FormGroup;
  viewConditionModelForm: FormGroup;
  viewLabTestModelForm: FormGroup;
  viewMedicationModelForm: FormGroup;
  viewOtherModelForm: FormGroup;

  facilityList: FacilityIdentifierModel[] = [];

  conditions: ConditionGridRecordModel[] = [];
  medications: MedicationGridRecordModel[] = [];
  labTests: LabTestGridRecordModel[] = [];

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.id = +self._activatedRoute.snapshot.paramMap.get('id');

    self.viewModelForm = self._formBuilder.group({
      formCompleted: [this.viewModel.formCompleted]
    });

    self.viewPatientModelForm = self._formBuilder.group({
      treatmentSiteId: [this.viewModel.treatmentSiteId, [conditionalValidator(() => this.viewModelForm.get('formCompleted').value, Validators.required, '')]],
      asmNumber: [this.viewModel.asmNumber, [Validators.required]],
      patientFirstName: [this.viewModel.patientFirstName, Validators.required],
      patientLastName: [this.viewModel.patientLastName, Validators.required],
      patientIdentityNumber: [this.viewModel.patientIdentityNumber, [Validators.required, Validators.maxLength(20)]],
      birthDate: [this.viewModel.birthDate],
      age: [this.viewModel.age],
      gender: [this.viewModel.gender],
      weight: [this.viewModel.weight, [Validators.max(199), Validators.min(0)]],
      pregnant: [this.viewModel.pregnant],
      lmpDate: [this.viewModel.lmpDate],
      gestAge: [this.viewModel.gestAge, [Validators.max(44), Validators.min(4)]],
      address: [this.viewModel.address, Validators.maxLength(100)],
      contactNumber: [this.viewModel.contactNumber, Validators.maxLength(15)],
      alcoholConsumption: [this.viewModel.alcoholConsumption],
      smoker: [this.viewModel.smoker],
      otherSubstance: [this.viewModel.otherSubstance],
    });

    this.viewModelForm.get('formCompleted').valueChanges
        .subscribe(value => {
            this.viewPatientModelForm.get('treatmentSiteId').updateValueAndValidity();
        });    

    self.viewConditionModelForm = self._formBuilder.group({
    });

    self.viewLabTestModelForm = self._formBuilder.group({
    });

    self.viewMedicationModelForm = self._formBuilder.group({
    });

    self.viewOtherModelForm = self._formBuilder.group({
      adherenceReason: [this.viewModel.adherenceReason],
      followUpDate: [this.viewModel.followUpDate|| '', Validators.required],
      nameReporter: [this.viewModel.nameReporter],
      currentDate: [this.viewModel.currentDate || moment(), Validators.required],
      telephoneReporter: [this.viewModel.telephoneReporter],
      professionReporter: [this.viewModel.professionReporter]
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.id > 0) {
      self.loadData();
    }
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormAComponent.name);
  }    

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getForm(self.id).then(result => {
        let form = result as Form;
        
        self.identifier = form.formIdentifier;

        self.updateForm(self.viewModelForm, JSON.parse(form.formValues[0].formControlValue));
        self.updateForm(self.viewPatientModelForm, JSON.parse(form.formValues[1].formControlValue));
        self.updateForm(self.viewOtherModelForm, JSON.parse(form.formValues[5].formControlValue));

        self.viewModel.conditionGrid.updateBasic(JSON.parse(form.formValues[2].formControlValue));
        self.viewModel.labTestGrid.updateBasic(JSON.parse(form.formValues[3].formControlValue));
        self.viewModel.medicationGrid.updateBasic(JSON.parse(form.formValues[4].formControlValue));
      
        self.conditions = JSON.parse(form.formValues[2].formControlValue);
        self.labTests = JSON.parse(form.formValues[3].formControlValue);
        self.medications = JSON.parse(form.formValues[4].formControlValue);

        self.isComplete = form.completeStatus == 'Complete';
        self.isSynched = form.synchStatus == 'Synched';

        if(self.isComplete || self.isSynched) {
          self.viewPatientModelForm.disable();
          self.viewConditionModelForm.disable();
          self.viewLabTestModelForm.disable();
          self.viewMedicationModelForm.disable();
          self.viewOtherModelForm.disable();
        }

        self.setBusy(false);
    }, error => {
          self.throwError(error, error.statusText);
    });
  }

  openConditionPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Condition' : 'Update Condition';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormAConditionsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        let condition: ConditionGridRecordModel = {
          index: isNew ? this.conditions.length : data.index,
          condition: res.condition,
          conditionStatus: res.conditionStatus
        };
        if(isNew) {
          this.conditions.push(condition);
        }
        else {
          this.conditions[data.index] = condition;
        }
        this.viewModel.conditionGrid.updateBasic(this.conditions);
        this.viewConditionModelForm.reset();
      })
  }  

  openLabPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Test Result' : 'Update Test Result';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormALabsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        let labTest: LabTestGridRecordModel = {
          index: isNew ? this.labTests.length : data.index,
          labTest: res.labTest,
          testResultDate: res.labTestDate.format('YYYY-MM-DD'),
          testResultValue: res.labTestResult
        };
        if(isNew) {
          this.labTests.push(labTest);
        }
        else {
          this.labTests[data.index] = labTest;
        }
        this.viewModel.labTestGrid.updateBasic(this.labTests);
        this.viewLabTestModelForm.reset();
      })
  }  

  openMedicationPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Medication' : 'Update Medication';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormAMedicationsPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        let medicationEndDate = '';
        if(moment.isMoment(res.medicationEndDate)) {
          medicationEndDate = res.medicationEndDate.format('YYYY-MM-DD');
        }
    
        let medication: MedicationGridRecordModel = {
          index: isNew ? this.medications.length : data.index,
          medication: res.medication,
          dose: res.dose,
          frequency: res.frequency,
          startDate: res.medicationStartDate.format('YYYY-MM-DD'),
          endDate: medicationEndDate,
          continued: res.continued
        };
        if(isNew) {
          this.medications.push(medication);
        }
        else {
          this.medications[data.index] = medication;
        }
        this.viewModel.medicationGrid.updateBasic(this.medications);
        this.viewMedicationModelForm.reset();
      })
  }

  openCompletePopup(formId: number) {
    let self = this;
    let title = "Form Completed";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormCompletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        self._router.navigate([_routes.clinical.forms.landing]);        
      })
  } 
  
  openGuidelinesPopup() {
    let self = this;
    let title = "GUIDELINES FOR COMPLETING THE TREATMENT INITIATION FORM (FORM A)";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormGuidelinesPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { title: title, type: 'A' }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
      })
  }  

  completeForm(): void {
    let self = this;
    let otherModels:any[]; 
    let attachments:FormAttachmentModel[] = []; 

    otherModels = [this.conditions, this.labTests, this.medications, this.viewOtherModelForm.value];

    if (self.id == 0) {
      self.metaFormService.saveFormToDatabase('FormA', this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form A saved successfully!', 'Form Saved');
                this.openCompletePopup(response);
              }
            else {
                self.showError('There was an error saving form A, please try again !', 'Download');
            }
        });
      }
      else {
        self.metaFormService.updateForm(self.id, this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
          {
              if (response) {
                  self.notify('Form A updated successfully!', 'Form Saved');
                  this.openCompletePopup(self.id);
                }
              else {
                  self.showError('There was an error updating form C, please try again !', 'Download');
              }
          });         
      }
  }

  saveForm(): void {
    let self = this;
    let otherModels:any[];
    let attachments:FormAttachmentModel[] = []; 

    otherModels = [this.conditions, this.labTests, this.medications, this.viewOtherModelForm.value];

    if (self.id == 0) {
      self.metaFormService.saveFormToDatabase('FormA', this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form A saved successfully!', 'Form Saved');
                self._router.navigate([_routes.clinical.forms.landing]);
            }
            else {
                self.showError('There was an error saving form A, please try again !', 'Download');
            }
        });
      }
      else {
        self.metaFormService.updateForm(self.id, this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
          {
              if (response) {
                  self.notify('Form A updated successfully!', 'Form Saved');
                  self._router.navigate([_routes.clinical.forms.landing]);
              }
              else {
                  self.showError('There was an error updating form C, please try again !', 'Download');
              }
          });         
      }
  }

  getFacilityList(): void {
    let self = this;
    self.facilityService.getAllFacilities()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.facilityList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }  

  removeMedication(index: number): void {
    let self = this;
    self.medications.splice(index, 1)
    this.viewModel.medicationGrid.updateBasic(this.medications);

    this.notify("Medication removed successfully!", "Medication");
  }  

  removeCondition(index: number): void {
    let self = this;
    self.conditions.splice(index, 1)
    this.viewModel.conditionGrid.updateBasic(this.conditions);

    this.notify("Condition removed successfully!", "Condition");
  }  

  removeLabTest(index: number): void {
    let self = this;
    self.labTests.splice(index, 1)
    this.viewModel.labTestGrid.updateBasic(this.labTests);

    this.notify("Lab test removed successfully!", "Lab Test");
  }
}

class ViewModel {
  formCompleted: any;

  treatmentSiteId: string;
  asmNumber: string;
  patientFirstName: string;
  patientLastName: string;
  patientIdentityNumber: string;
  birthDate: Moment;
  age: number;
  gender: string;
  weight: number;
  pregnant: string;
  lmpDate: Moment;
  gestAge: number;
  address: string;
  contactNumber: string;
  alcoholConsumption: string;
  smoker: string;
  otherSubstance: string;

  conditionGrid: GridModel<ConditionGridRecordModel> =
  new GridModel<ConditionGridRecordModel>
      (['condition', 'condition status', 'actions']);

  labTestGrid: GridModel<LabTestGridRecordModel> =
  new GridModel<LabTestGridRecordModel>
      (['lab test', 'test date', 'test result', 'actions']);

  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'start date', 'continued', 'actions']);

  adherenceReason: string;
  followUpDate: Moment;
  nameReporter: string;
  currentDate: Moment;
  telephoneReporter: string;
  professionReporter: string;
}

class ConditionGridRecordModel {
  index: number;
  condition: string;
  conditionStatus: string;
}

class LabTestGridRecordModel {
  index: number;
  labTest: string;
  testResultDate: string;
  testResultValue: string;
}

class MedicationGridRecordModel {
  index: number;
  medication: string;
  dose: string;
  frequency: string;
  startDate: string;
  endDate: string;
  continued: string;
}

export interface BooleanFn {
  (): boolean;
}

/**
 * A conditional validator generator. Assigns a validator to the form control if the predicate function returns true on the moment of validation
 * @example
 * Here if the myCheckbox is set to true, the myEmailField will be required and also the text will have to have the word 'mason' in the end.
 * If it doesn't satisfy these requirements, the errors will placed to the dedicated `illuminatiError` namespace.
 * Also the myEmailField will always have `maxLength`, `minLength` and `pattern` validators.
 * ngOnInit() {
 *   this.myForm = this.fb.group({
 *    myCheckbox: [''],
 *    myEmailField: ['', [
 *       Validators.maxLength(250),
 *       Validators.minLength(5),
 *       Validators.pattern(/.+@.+\..+/),
 *       conditionalValidator(() => this.myForm.get('myCheckbox').value,
 *                            Validators.compose([
 *                            Validators.required,
 *                            Validators.pattern(/.*mason/)
 *         ]),
 *        'illuminatiError')
 *        ]]
 *     })
 * }
 * @param predicate
 * @param validator
 * @param errorNamespace optional argument that creates own namespace for the validation error
 */
export function conditionalValidator(predicate: BooleanFn,
                validator: ValidatorFn,
                errorNamespace?: string): ValidatorFn {
  return (formControl => {
    if (!formControl.parent) {
      return null;
    }
    let error = null;
    if (predicate()) {
      error = validator(formControl);
    }
    if (errorNamespace && error) {
      const customError = {};
      customError[errorNamespace] = error;
      error = customError
    }
    return error;
  })
}