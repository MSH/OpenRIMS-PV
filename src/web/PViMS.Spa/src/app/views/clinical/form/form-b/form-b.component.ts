import { Component, OnInit, AfterViewInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription, Observable } from 'rxjs';
import { FacilityService } from 'app/shared/services/facility.service';
import { takeUntil } from 'rxjs/operators';
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
import { FormBConditionsPopupComponent } from './form-b-conditions-popup/form-b-conditions.popup.component';
import { FormBLabsPopupComponent } from './form-b-labs-popup/form-b-labs.popup.component';
import { FormBMedicationsPopupComponent } from './form-b-medications-popup/form-b-medications.popup.component';
import { FormBAdversePopupComponent } from './form-b-adverse-popup/form-b-adverse.popup.component';
import { FormCompletePopupComponent } from '../form-complete-popup/form-complete.popup.component';
import { MeddraTermIdentifierModel } from 'app/shared/models/terminology/meddra-term.identifier.model';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';
import { FormGuidelinesPopupComponent } from '../form-guidelines-popup/form-guidelines.popup.component';

const moment =  _moment;

@Component({
  selector: 'app-form-a',
  templateUrl: './form-b.component.html',
  styleUrls: ['./form-b.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormBComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected facilityService: FacilityService,
    protected metaFormService: MetaFormService,
    protected dialog: MatDialog,
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
  viewPregnancyModelForm: FormGroup;
  viewMedicationModelForm: FormGroup;
  viewAdverseEventModelForm: FormGroup;
  viewConditionModelForm: FormGroup;
  viewAdverseEventManagementModelForm: FormGroup;
  viewLabTestModelForm: FormGroup;
  viewOutcomeModelForm: FormGroup;

  facilityList: FacilityIdentifierModel[] = [];
  filteredAdverseEventMeddraTermList: Observable<MeddraTermIdentifierModel[]>;

  medications: MedicationGridRecordModel[] = [];
  adverseEvents: AdverseEventGridRecordModel[] = []; 
  conditions: ConditionGridRecordModel[] = [];
  labTests: LabTestGridRecordModel[] = [];

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.id = +self._activatedRoute.snapshot.paramMap.get('id');

    self.viewModelForm = self._formBuilder.group({
      formCompleted: [this.viewModel.formCompleted]
    });

    self.viewPatientModelForm = self._formBuilder.group({
      treatmentSiteId: [this.viewModel.treatmentSiteId, Validators.required],
      patientIdentityNumber: [this.viewModel.patientIdentityNumber, [Validators.required, Validators.maxLength(20)]],
      asmNumber: [this.viewModel.asmNumber, [Validators.required, Validators.maxLength(50)]],
      patientFirstName: [this.viewModel.patientFirstName],
      patientLastName: [this.viewModel.patientLastName],
      birthDate: [this.viewModel.birthDate],
      age: [this.viewModel.age | 0],
      gender: [this.viewModel.gender],
      weight: [this.viewModel.weight, [Validators.max(199), Validators.min(0)]],
    });

    self.viewPregnancyModelForm = self._formBuilder.group({
      gravidity: [this.viewModel.gravidity, [Validators.max(29), Validators.min(0)]],
      parity: [this.viewModel.parity, [Validators.max(29), Validators.min(0)]],
      stillPregnant: [this.viewModel.stillPregnant],
      gestAge: [this.viewModel.gestAge, [Validators.max(44), Validators.min(4)]],
      spontAbortion: [this.viewModel.spontAbortion],
      spontAbortionDate: [this.viewModel.spontAbortionDate],
      inducedAbortion: [this.viewModel.inducedAbortion],
      inducedAbortionDate: [this.viewModel.inducedAbortionDate],
      stillLife: [this.viewModel.stillLife],
      stillLifeDate: [this.viewModel.stillLifeDate],
      liveBirth: [this.viewModel.liveBirth],
      liveBirthDate: [this.viewModel.liveBirthDate],
    });

    self.viewMedicationModelForm = self._formBuilder.group({
    });

    self.viewAdverseEventModelForm = self._formBuilder.group({
      hasNewAdverseEvent: [this.viewModel.hasNewAdverseEvent],
    });

    self.viewConditionModelForm = self._formBuilder.group({
      hasConditionChange: [this.viewModel.hasConditionChange],
    });

    self.viewAdverseEventManagementModelForm = self._formBuilder.group({
      adverseEventTreatment: [this.viewModel.adverseEventTreatment],
      adverseEventTreatmentDetail: [this.viewModel.adverseEventTreatmentDetail],
      adverseEventOutcome: [this.viewModel.adverseEventOutcome],
    });

    self.viewLabTestModelForm = self._formBuilder.group({
    });

    self.viewOutcomeModelForm = self._formBuilder.group({
      visitOutcome: [this.viewModel.visitOutcome],
      nameReporter: [this.viewModel.nameReporter],
      currentDate: [this.viewModel.currentDate || moment(), Validators.required],
      telephoneReporter: [this.viewModel.telephoneReporter]
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.medicationGrid.setupBasic(null, null, null);
    self.viewModel.adverseEventGrid.setupBasic(null, null, null);
    self.viewModel.conditionGrid.setupBasic(null, null, null);

    if (self.id > 0) {
      self.loadData();
    }
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormBComponent.name);
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
        self.updateForm(self.viewPregnancyModelForm, JSON.parse(form.formValues[2].formControlValue));
        self.updateForm(self.viewAdverseEventModelForm, JSON.parse(form.formValues[4].formControlValue));
        self.updateForm(self.viewConditionModelForm, JSON.parse(form.formValues[6].formControlValue));
        self.updateForm(self.viewAdverseEventManagementModelForm, JSON.parse(form.formValues[8].formControlValue));
        self.updateForm(self.viewOutcomeModelForm, JSON.parse(form.formValues[10].formControlValue));

        self.viewModel.medicationGrid.updateBasic(JSON.parse(form.formValues[3].formControlValue));
        self.viewModel.adverseEventGrid.updateBasic(JSON.parse(form.formValues[5].formControlValue));
        self.viewModel.conditionGrid.updateBasic(JSON.parse(form.formValues[7].formControlValue));
        self.viewModel.labTestGrid.updateBasic(JSON.parse(form.formValues[9].formControlValue));

        self.medications = JSON.parse(form.formValues[3].formControlValue);
        self.adverseEvents = JSON.parse(form.formValues[5].formControlValue);
        self.conditions = JSON.parse(form.formValues[7].formControlValue);
        self.labTests = JSON.parse(form.formValues[9].formControlValue);

        self.isComplete = form.completeStatus == 'Complete';
        self.isSynched = form.synchStatus == 'Synched';

        if(self.isComplete || self.isSynched) {
          self.viewPatientModelForm.disable();
          self.viewPregnancyModelForm.disable();
          self.viewMedicationModelForm.disable();
          self.viewAdverseEventModelForm.disable();
          self.viewConditionModelForm.disable();
          self.viewAdverseEventManagementModelForm.disable();
          self.viewLabTestModelForm.disable();
          self.viewOutcomeModelForm.disable();
        }

        self.setBusy(false);
    }, error => {
          self.throwError(error, error.statusText);
    });
  }

  openConditionPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Condition' : 'Update Condition';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormBConditionsPopupComponent, {
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
        let conditionStartDate = '';
        if(moment.isMoment(res.conditionStartDate)) {
          conditionStartDate = res.conditionStartDate.format('YYYY-MM-DD');
        }
        let conditionEndDate = '';
        if(moment.isMoment(res.conditionEndDate)) {
          conditionEndDate = res.conditionEndDate.format('YYYY-MM-DD');
        }

        let condition: ConditionGridRecordModel = {
          index: isNew ? this.conditions.length : data.index,
          condition: res.condition,
          conditionStatus: res.conditionStatus,
          startDate: conditionStartDate,
          endDate: conditionEndDate,
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
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormBLabsPopupComponent, {
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
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormBMedicationsPopupComponent, {
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

  openAdversePopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Adverse Event' : 'Update Adverse Event';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormBAdversePopupComponent, {
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
    
        let adverseEventEndDate = '';
        if(moment.isMoment(res.adverseEventEndDate)) {
          adverseEventEndDate = res.adverseEventEndDate.format('YYYY-MM-DD');
        }
    
        let adverseEvent: AdverseEventGridRecordModel = {
          index: isNew ? this.adverseEvents.length : data.index,
          adverseEvent: res.adverseEvent,
          startDate: res.adverseEventStartDate.format('YYYY-MM-DD'),
          endDate: adverseEventEndDate,
          gravity: res.gravity,
          serious: res.serious,
          severity: res.severity
        };
        if(isNew) {
          this.adverseEvents.push(adverseEvent);
        }
        else {
          this.adverseEvents[data.index] = adverseEvent;
        }
        this.viewModel.adverseEventGrid.updateBasic(this.adverseEvents);
        this.viewAdverseEventModelForm.reset();
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
    let title = "GUIDELINES FOR COMPLETING THE FOLLOWUP FORM (FORM B)";
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

    otherModels = [this.viewPregnancyModelForm.value, this.medications, this.viewAdverseEventModelForm.value, this.adverseEvents, this.viewConditionModelForm.value, this.conditions, this.viewAdverseEventManagementModelForm.value, this.labTests, this.viewOutcomeModelForm.value];

    if (self.id == 0) {
      self.metaFormService.saveFormToDatabase('FormB', this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form B saved successfully!', 'Form Saved');
                this.openCompletePopup(response);
            }
            else {
                self.showError('There was an error saving form B, please try again !', 'Download');
            }
        });
    }
    else {
      self.metaFormService.updateForm(self.id, this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form B updated successfully!', 'Form Saved');
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

    otherModels = [this.viewPregnancyModelForm.value, this.medications, this.viewAdverseEventModelForm.value, this.adverseEvents, this.viewConditionModelForm.value, this.conditions, this.viewAdverseEventManagementModelForm.value, this.labTests, this.viewOutcomeModelForm.value];

    if (self.id == 0) {
      self.metaFormService.saveFormToDatabase('FormB', this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form B saved successfully!', 'Form Saved');
                self._router.navigate([_routes.clinical.forms.landing]);
            }
            else {
                self.showError('There was an error saving form B, please try again !', 'Download');
            }
        });
    }
    else {
      self.metaFormService.updateForm(self.id, this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form B updated successfully!', 'Form Saved');
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

  removeAdverseEvent(index: number): void {
    let self = this;
    self.adverseEvents.splice(index, 1)
    this.viewModel.adverseEventGrid.updateBasic(this.adverseEvents);

    this.notify("Adverse event removed successfully!", "Adverse Event");
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
  patientIdentityNumber: string;
  patientFirstName: string;
  patientLastName: string;
  birthDate: Moment;
  age: number;
  gender: string;
  weight: number;

  gravidity: number;
  parity: number;
  stillPregnant: string;
  gestAge: number;
  spontAbortion: string;
  spontAbortionDate: Moment;
  inducedAbortion: string;
  inducedAbortionDate: Moment;
  stillLife: string;
  stillLifeDate: Moment;
  liveBirth: string;
  liveBirthDate: Moment;

  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'start date', 'continued', 'actions']);      

  hasNewAdverseEvent: string;

  adverseEventGrid: GridModel<AdverseEventGridRecordModel> =
  new GridModel<AdverseEventGridRecordModel>
      (['adverse event', 'start date', 'gravity', 'actions']);      

  hasConditionChange: string;

  conditionGrid: GridModel<ConditionGridRecordModel> =
  new GridModel<ConditionGridRecordModel>
      (['condition', 'condition status', 'start date', 'actions']);

  adverseEventTreatment: string;
  adverseEventTreatmentDetail: string;
  adverseEventOutcome: string;

  labTestGrid: GridModel<LabTestGridRecordModel> =
  new GridModel<LabTestGridRecordModel>
      (['lab test', 'test date', 'test result', 'actions']);

  visitOutcome: string;
  nameReporter: string;
  telephoneReporter: string;
  currentDate: Moment;
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

class AdverseEventGridRecordModel {
  index: number;
  adverseEvent: string;
  startDate: string;
  endDate: string;
  gravity: string;
  serious: string;
  severity: string;
}

class ConditionGridRecordModel {
  index: number;
  condition: string;
  conditionStatus: string;
  startDate: string;
  endDate: string;
}

class LabTestGridRecordModel {
  index: number;
  labTest: string;
  testResultDate: string;
  testResultValue: string;
}
