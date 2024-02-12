import { AfterViewInit, Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { EventService } from 'app/shared/services/event.service';
import { PatientService } from 'app/shared/services/patient.service';
import { concatMap, finalize, switchMap, takeUntil } from 'rxjs/operators';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AttachmentAddPopupComponent } from '../../shared/attachment-add-popup/attachment-add.popup.component';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';
import { GridModel } from 'app/shared/models/grid.model';
import { PatientMedicationForUpdateModel } from 'app/shared/models/patient/patient-medication-for-update.model';
import { PatientClinicalEventForUpdateModel } from 'app/shared/models/patient/patient-clinical-event-for-update.model';
import { forkJoin, from, Observable, of } from 'rxjs';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { _routes } from 'app/config/routes';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientMedicationDetailModel } from 'app/shared/models/patient/patient-medication.detail.model';
import { FormADRMedicationPopupComponent } from '../../shared/form-adr-medication-popup/form-adr-medication.popup.component';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { Form } from 'app/shared/indexed-db/appdb';
import { FormCompletePopupComponent } from '../form-complete-popup/form-complete.popup.component';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';

const moment =  _moment;

@Component({
  templateUrl: './form-adr.component.html',
  styles: [`
    .mat-column-file-name { flex: 0 0 85% !important; width: 85% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],   
  animations: egretAnimations
})
export class FormADRComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {
  // @HostListener allows us to also guard against browser refresh, close, etc.
  @HostListener('window:beforeunload')
  
  public scrollConfig = {}

  viewModel: ViewModel = new ViewModel();

  public viewModelForm: FormGroup;
  public firstFormGroup: FormGroup;
  public thirdFormGroup: FormGroup;
  public fourthFormGroup: FormGroup;
  public fifthFormGroup: FormGroup;
  public sixthFormGroup: FormGroup;

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService,
    protected metaFormService: MetaFormService,
    protected dialog: MatDialog) 
  { 
    super(_router, _location, popupService, accountService, eventService);
  }

  canDeactivate(): Observable<boolean> | boolean {
    // returning true will navigate without confirmation
    // returning false will show a confirm dialog before navigating away
    if(this.firstFormGroup.dirty ||
      this.thirdFormGroup.dirty ||
      this.fourthFormGroup.dirty ||
      this.fifthFormGroup.dirty ||
      this.sixthFormGroup.dirty) {
        return false;
    }
    return true;
  }  

  ngOnInit(): void {
    const self = this;

    self.viewModel.formId = +self._activatedRoute.snapshot.paramMap.get('id');
    
    self.viewModelForm = self._formBuilder.group({
      formCompleted: ['']
    });
    self.firstFormGroup = this._formBuilder.group({
      caseNumber: ['', Validators.required],
      patientId: [''],
      patientIdentifier: [''],
      patientFirstName: [''],
      patientLastName: [''],
      gender: [''],
      ethnicity: [''],
      dateOfBirth: [''],
      age: [''],
      ageGroup: [''],
      facilityName: [''],
      facilityRegion: [''],
    });
    self.thirdFormGroup = this._formBuilder.group({
      dateOfOnset: ['', Validators.required],
      regimen: [null, Validators.required],
      sourceDescription: [null, [Validators.required, Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., '/\n/\r/\t/\s]*")]],
      isSerious: [null, Validators.required],
      seriousness: [null, Validators.required],
      classification: [null, Validators.required],
      weight: [null, [Validators.required, Validators.min(0), Validators.max(159)]],
      height: [null, [Validators.required, Validators.min(1), Validators.max(259)]],
      allergy: ['', [Validators.required, Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      pregnancyStatus: [null, Validators.required],
      comorbidities: ['', [Validators.required, Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
    });
    self.fourthFormGroup = this._formBuilder.group({
      treatmentGiven: [null],
      treatmentDetails: ['', [Validators.maxLength(300), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      outcome: [null, Validators.required],
      dateOfRecovery: [''],
      sequlae: ['', [Validators.maxLength(300), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      interventions: ['', [Validators.maxLength(300), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
    });
    self.fifthFormGroup = this._formBuilder.group({
    });
    self.sixthFormGroup = this._formBuilder.group({
      reporterName: ['', [Validators.required, Validators.maxLength(100), Validators.pattern("[-a-zA-Z ']*")]],
      contactNumber: ['', [Validators.maxLength(30), Validators.pattern("[-0-9+']*")]],
      emailAddress: ['', [Validators.required, Validators.maxLength(100)]],
      profession: [null]
    });

    self.getCustomAttributeList();
    self.accountService.connected$.subscribe(val => {
      self.viewModel.connected = val;
    });
    
    self.viewModel.attachmentGrid.setupBasic(null, null, null);    
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.viewModel.formId > 0) {
       self.loadFormData();
    }
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormADRComponent.name);
  }   

  nextStep(): void {
    this.viewModel.currentStep ++;
  }

  previousStep(): void {
    this.viewModel.currentStep --;
  }

  loadFormData(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getForm(self.viewModel.formId).then(result => {
        let form = result as Form;
        
        self.viewModel.formIdentifier = form.formIdentifier;

        self.updateForm(self.viewModelForm, JSON.parse(form.formValues[0].formControlValue));
        self.updateForm(self.firstFormGroup, JSON.parse(form.formValues[1].formControlValue));
        self.updateForm(self.thirdFormGroup, JSON.parse(form.formValues[2].formControlValue));
        self.updateForm(self.fourthFormGroup, JSON.parse(form.formValues[3].formControlValue));
        self.updateForm(self.sixthFormGroup, JSON.parse(form.formValues[5].formControlValue));

        self.viewModel.medications = JSON.parse(form.formValues[4].formControlValue);
        self.viewModel.medicationGrid.updateBasic(self.viewModel.medications);

        form.formAttachments.forEach(attachment => {
          self.viewModel.attachments.push({ description: attachment.description, file: attachment.file })  
        });
        self.viewModel.attachmentGrid.updateBasic(self.viewModel.attachments);            

        self.viewModel.patientFound = true;
        self.viewModel.isComplete = form.completeStatus == 'Complete';
        self.viewModel.isSynched = form.synchStatus == 'Synched';

        if(self.viewModel.isSynched) {
          self.firstFormGroup.disable();
          self.thirdFormGroup.disable();
          self.fourthFormGroup.disable();
          self.sixthFormGroup.disable();
        }
        self.setBusy(false);
    }, error => {
          self.throwError(error, error.statusText);
    });
  }  

  getPatient(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientByCondition(self.firstFormGroup.value)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        if(result == null) {
          self.viewModel.errorFindingPatient = true;
          self.viewModel.patientFound = false;
        }
        else {
          self.viewModel.patientId = result.id;
          self.viewModel.errorFindingPatient = false;
          self.viewModel.patientFound = true;

          self.updateForm(self.firstFormGroup, {patientId: result.id});
          self.updateForm(self.firstFormGroup, {patientFirstName: result.firstName});
          self.updateForm(self.firstFormGroup, {patientLastName: result.lastName});
          self.updateForm(self.firstFormGroup, {dateOfBirth: result.dateOfBirth});
          self.updateForm(self.firstFormGroup, {age: result.age});
          self.updateForm(self.firstFormGroup, {ageGroup: result.ageGroup});
          self.updateForm(self.firstFormGroup, {patientIdentifier: self.firstFormGroup.get('caseNumber').value});
          self.updateForm(self.firstFormGroup, {gender: self.getValueOrSelectedValueFromAttribute(result.patientAttributes, "Gender")});
          self.updateForm(self.firstFormGroup, {ethnicity: self.getValueOrSelectedValueFromAttribute(result.patientAttributes, "Ethnic Group")});
          self.updateForm(self.firstFormGroup, {facilityName: result.facilityName});
          self.updateForm(self.firstFormGroup, {facilityRegion: result.organisationUnit});

          self.viewModel.medications = self.mapMedicationForUpdateModels(result.patientMedications);
          self.viewModel.medicationGrid.updateBasic(self.viewModel.medications);
        }
      }, error => {
        self.handleError(error, "Error fetching patient");
      });
  }

  getCustomAttributeList(): void {
    let self = this;

    const requestArray = [];

    requestArray.push(self.customAttributeService.getAllCustomAttributes('PatientClinicalEvent'));
    requestArray.push(self.customAttributeService.getAllCustomAttributes('Patient'));

    forkJoin(requestArray)
      .pipe(
        switchMap((values: any[]) => {
          const mergeAttributeList: CustomAttributeDetailModel[] = [];

          values[0].forEach((attribute) => {
            mergeAttributeList.push(attribute);
          });
          values[1].forEach((attribute) => {
            mergeAttributeList.push(attribute);
          });

          return of(mergeAttributeList);
        })
      )
      .subscribe(
        data => {
          self.viewModel.customAttributeList = data;
        },
        error => {
          this.handleError(error, "Error loading attributes");
        });    
  }  

  openAttachmentPopUp() {
    let self = this;
    let title = 'Add Attachment';
    let dialogRef: MatDialogRef<any> = self.dialog.open(AttachmentAddPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.viewModel.attachments.push(res);
        self.viewModel.attachmentGrid.updateBasic(self.viewModel.attachments);
      })
  }

  removeAttachment(index: number): void {
    let self = this;
    self.viewModel.attachments.splice(index, 1)
    self.viewModel.attachmentGrid.updateBasic(self.viewModel.attachments);    

    this.notify("Attachment removed successfully!", "Success");
  }
  
  openMedicationPopup(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Medication' : 'Update Medication';
    let indexToUse = isNew ? self.viewModel.medications.length + 1 : data.index;
    
    let existingMedication = null;
    if (!isNew) {
      let actualIndex = self.viewModel.medications.findIndex(m => m.index == indexToUse);
      existingMedication = self.viewModel.medications[actualIndex];
    }
    
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormADRMedicationPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { title: title, medicationId: isNew ? 0: existingMedication.id, index: indexToUse, existingMedication }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        if(isNew) {
          self.viewModel.medications.push(res);
        }
        else {
          let actualIndex = self.viewModel.medications.findIndex(m => m.index == indexToUse);
          self.viewModel.medications[actualIndex] = res;
        }
    
        self.viewModel.medicationGrid.updateBasic(self.viewModel.medications);
        self.fifthFormGroup.reset();
      })
  }

  removeMedication(index: number): void {
    let self = this;

    let actualIndex = self.viewModel.medications.findIndex(m => m.index == index);
    self.viewModel.medications.splice(actualIndex, 1)
    this.viewModel.medicationGrid.updateBasic(self.viewModel.medications);

    this.notify("Medication removed successfully!", "Medication");
  }

  saveFormOnline(): void {
    const self = this;
    self.viewModel.saving = true;
    from(self.viewModel.medications).pipe(
      concatMap(medicationForUpdate => self.patientService.savePatientMedication(self.viewModel.patientId, medicationForUpdate.id, medicationForUpdate))
    ).pipe(
      finalize(() => self.saveOnlineMedicationsComplete()),
    ).subscribe(
      data => {
        self.CLog('subscription to save meds');
      },
      error => {
        this.handleError(error, "Error saving medications");
      });    
  }
  
  saveFormOffline(): void {
    const self = this;
    let otherModels:any[]; 
    otherModels = [self.thirdFormGroup.value, self.fourthFormGroup.value, self.viewModel.medications, self.sixthFormGroup.value];

    if (self.viewModel.formId == 0) {
      self.metaFormService.saveFormToDatabase('FormADR', self.viewModelForm.value, self.firstFormGroup.value, self.viewModel.attachments, otherModels).then(response =>
        {
          if (response) {
            self.setBusy(false);              
            self.notify('Form saved successfully!', 'Form Saved');

            self.firstFormGroup.markAsPristine();
            self.thirdFormGroup.markAsPristine();
            self.fourthFormGroup.markAsPristine();
            self.fifthFormGroup.markAsPristine();
            self.sixthFormGroup.markAsPristine();
    
            self._router.navigate([_routes.clinical.forms.landing]);
          }
          else {
            self.showError('There was an error saving the form locally, please try again !', 'Form Error');
          }
        });
      }
      else {
        self.metaFormService.updateForm(self.viewModel.formId, this.viewModelForm.value, this.firstFormGroup.value, self.viewModel.attachments, otherModels).then(response =>
          {
              if (response) {
                  self.notify('Form updated successfully!', 'Form Saved');

                  self.firstFormGroup.markAsPristine();
                  self.thirdFormGroup.markAsPristine();
                  self.fourthFormGroup.markAsPristine();
                  self.fifthFormGroup.markAsPristine();
                  self.sixthFormGroup.markAsPristine();
      
                  self._router.navigate([_routes.clinical.forms.landing]);
              }
              else {
                  self.showError('There was an error saving the form locally, please try again !', 'Form Error');
              }
          });
      }
  }

  completeFormOffline(): void {
    let self = this;
    let otherModels:any[];

    otherModels = [self.thirdFormGroup.value, self.fourthFormGroup.value, self.viewModel.medications, self.sixthFormGroup.value];

    if (self.viewModel.formId == 0) {
      self.metaFormService.saveFormToDatabase('FormADR', self.viewModelForm.value, self.firstFormGroup.value, self.viewModel.attachments, otherModels).then(response =>
        {
            if (response) {
              self.setBusy(false);              
              self.notify('Form saved successfully!', 'Form Saved');
  
              self.firstFormGroup.markAsPristine();
              self.thirdFormGroup.markAsPristine();
              self.fourthFormGroup.markAsPristine();
              self.fifthFormGroup.markAsPristine();
              self.sixthFormGroup.markAsPristine();
      
              self.openCompletePopup(+response);
            }
            else {
                self.showError('There was an error saving the form locally, please try again !', 'Form Error');
            }
        });
      }
      else {
        self.metaFormService.updateForm(self.viewModel.formId, this.viewModelForm.value, this.firstFormGroup.value, self.viewModel.attachments, otherModels).then(response =>
          {
              if (response) {
                  self.notify('Form updated successfully!', 'Form Saved');

                  self.firstFormGroup.markAsPristine();
                  self.thirdFormGroup.markAsPristine();
                  self.fourthFormGroup.markAsPristine();
                  self.fifthFormGroup.markAsPristine();
                  self.sixthFormGroup.markAsPristine();
    
                  this.openCompletePopup(self.viewModel.formId);
                }
              else {
                  self.showError('There was an error updating the form locally, please try again !', 'Form Error');
              }
          });
      }
  } 

  private saveOnlineMedicationsComplete(): void {
    const self = this;
    const requestArray = [];

    var clinicalEventForUpdate = self.prepareClinicalEventForUpdateModel();
    requestArray.push(this.patientService.savePatientClinicalEvent(self.viewModel.patientId, 0, clinicalEventForUpdate));

    self.viewModel.attachments.forEach(attachmentForUpdate => {
      requestArray.push(this.patientService.saveAttachment(self.viewModel.patientId, attachmentForUpdate.file, attachmentForUpdate.description));
    });

    forkJoin(requestArray)
    .subscribe(
      data => {
        self.setBusy(false);
        self.notify('Form added successfully!', 'Success');

        self.firstFormGroup.markAsPristine();
        self.thirdFormGroup.markAsPristine();
        self.fourthFormGroup.markAsPristine();
        self.fifthFormGroup.markAsPristine();
        self.sixthFormGroup.markAsPristine();

        self._router.navigate([_routes.clinical.forms.landing]);
      },
      error => {
        this.handleError(error, "Error adding form");
      });
  }
    
  private openCompletePopup(formId: number) {
    let self = this;
    let title = "Form Completed";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormCompletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId, title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }        
        self._router.navigate([_routes.clinical.forms.landing]);        
      })
  }    

  private prepareClinicalEventForUpdateModel(): PatientClinicalEventForUpdateModel {
    let self = this;
    let onsetDate = self.thirdFormGroup.get('dateOfOnset').value.format('YYYY-MM-DD');
    let resolutionDate = '';
    if(moment.isMoment(self.fourthFormGroup.get('dateOfRecovery').value)) {
      resolutionDate = self.fourthFormGroup.get('dateOfRecovery').value.format('YYYY-MM-DD');
    }
    else {
      if (self.fourthFormGroup.get('dateOfRecovery').value != '') {
        resolutionDate = self.fourthFormGroup.get('dateOfRecovery').value;
      }
    }

    const attributesForUpdate: AttributeValueForPostModel[] = [];

    attributesForUpdate.push(self.prepareAttributeValue('regimen', 'regimen', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('is the adverse event serious?', 'isSerious', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('seriousness', 'seriousness', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('classification', 'classification', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('weight (kg)', 'weight', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('height (cm)', 'height', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('any known allergy', 'allergy', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('pregnancy status', 'pregnancyStatus', self.thirdFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('comorbidities', 'comorbidities', self.thirdFormGroup));
    
    attributesForUpdate.push(self.prepareAttributeValue('was treatment given?', 'treatmentGiven', self.fourthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('treatment details', 'treatmentDetails', self.fourthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('outcome', 'outcome', self.fourthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('sequlae details', 'sequlae', self.fourthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('interventions', 'interventions', self.fourthFormGroup));

    attributesForUpdate.push(self.prepareAttributeValue('name of reporter', 'reporterName', self.sixthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('contact number', 'contactNumber', self.sixthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('email address', 'emailAddress', self.sixthFormGroup));
    attributesForUpdate.push(self.prepareAttributeValue('profession', 'profession', self.sixthFormGroup));

    const clinicalEventForUpdate: PatientClinicalEventForUpdateModel = 
    {
      id: 0,
      index: 1,
      patientIdentifier: self.firstFormGroup.get('patientIdentifier').value,
      onsetDate,
      sourceDescription: self.thirdFormGroup.get('sourceDescription').value,
      resolutionDate,
      sourceTerminologyMedDraId: null,
      attributes: attributesForUpdate
    };

    return clinicalEventForUpdate;
  }

  private prepareAttributeValue(attributeKey: string, formKey: string, sourceForm: FormGroup): AttributeValueForPostModel {
    const self = this;
    let customAttribute = self.viewModel.customAttributeList.find(ca => ca.attributeKey.toLowerCase() == attributeKey.toLowerCase());
    if(customAttribute == null) {
      return null;
    }

    const attributeForPost: AttributeValueForPostModel = {
      id: customAttribute.id,
      value: sourceForm.get(formKey).value
    }    
    return attributeForPost;
  }

  private mapMedicationForUpdateModels(sourceMedications: PatientMedicationDetailModel[]): PatientMedicationForUpdateModel[] {
    let medications: PatientMedicationForUpdateModel[] = [];

    let index = 0;
    sourceMedications.forEach(sourceMedication => {
      index++;
      let medication: PatientMedicationForUpdateModel = {
        id: sourceMedication.id,
        index,
        medication: sourceMedication.medication,
        sourceDescription: sourceMedication.sourceDescription,
        conceptId: sourceMedication.conceptId,
        productId: sourceMedication.productId,
        startDate: sourceMedication.startDate,
        endDate: sourceMedication.endDate,
        dose: sourceMedication.dose,
        doseFrequency: sourceMedication.doseFrequency,
        doseUnit: sourceMedication.doseUnit,
        attributes: []
      };
      
      sourceMedication.medicationAttributes.forEach(sourceAttribute => {
        let attribute: AttributeValueForPostModel = {
          id: sourceAttribute.id,
          value: sourceAttribute.value
        };
        medication.attributes.push(attribute);
      });

      medications.push(medication);
    });

    return medications;
  }
}

class ViewModel {
  formId: number;
  formIdentifier: string;
  
  patientId: number;
  patientFound = false;
  errorFindingPatient = false;

  currentStep = 1;
  isComplete = false;
  isSynched = false;
  connected: boolean = true;
  saving: boolean = false;

  customAttributeKey = 'Case Number';
  customAttributeList: CustomAttributeDetailModel[] = [];
 
  attachmentGrid: GridModel<FormAttachmentModel> =
    new GridModel<FormAttachmentModel>
        (['file-name', 'actions']);
  attachments: FormAttachmentModel[] = [];

  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'start-date', 'end-date', 'dose', 'actions']);
  medications: PatientMedicationForUpdateModel[] = [];
}

class MedicationGridRecordModel {
  id: number;
  index: number;
  medication: string;
  dose: string;
  doseUnit: string;
  startDate: string;
  endDate: string;
}