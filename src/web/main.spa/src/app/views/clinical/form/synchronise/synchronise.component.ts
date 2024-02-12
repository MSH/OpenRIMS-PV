import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { forkJoin, from, of, Subscription } from 'rxjs';
import { GridModel } from 'app/shared/models/grid.model';
import { _routes } from 'app/config/routes';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ViewErrorPopupComponent } from './viewerror-popup/viewerror.popup.component';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { concatMap, finalize, switchMap } from 'rxjs/operators';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { Form } from 'app/shared/indexed-db/appdb';
import { PatientService } from 'app/shared/services/patient.service';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';
import { PatientMedicationForUpdateModel } from 'app/shared/models/patient/patient-medication-for-update.model';
import { PatientClinicalEventForUpdateModel } from 'app/shared/models/patient/patient-clinical-event-for-update.model';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';

const moment =  _moment;

@Component({
  templateUrl: './synchronise.component.html',
  styles: [`
    .mat-column-selected { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }
  `],   
  animations: egretAnimations
})
export class SynchroniseComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected metaFormService: MetaFormService,
    protected customAttributeService: CustomAttributeService,
    protected patientService: PatientService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog,) 
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

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  ngOnInit(): void {
    const self = this;

    self.viewModel.formType = self._activatedRoute.snapshot.paramMap.get('type');

    self.getCustomAttributeList();
    self.accountService.connected$.subscribe(val => {
        self.viewModel.connected = val;
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupBasic(null, null, null);

    self.loadGrid();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(SynchroniseComponent.name);
  }

  selectToggleAll() {
    const self = this;
    self.viewModel.selectToggleFlag = !self.viewModel.selectToggleFlag;
    self.viewModel.mainGrid.records.data.forEach(form => { form.selected = self.viewModel.selectToggleFlag });
  }

  getCustomAttributeList(): void {
    const self = this;

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

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getFilteredFormsByType(self.viewModel.formType, false, true).then(result => {
      self.viewModel.mainGrid.updateBasic(result.value);
      self.setBusy(false);
    }, error => {
      self.throwError(error, error.statusText);
    });
  }  

  hasAtLeastOneSelection() : boolean {
    const self = this;
    return (this.viewModel.mainGrid.records.data.filter(form => form.selected).length > 0)
  }

  viewForm(model: GridRecordModel = null): void {
    const self = this;
    console.log(model);
    switch(model.formType) { 
      case 'FormA': { 
        self._router.navigate([_routes.clinical.forms.viewFormA(model.id)]);
         break; 
      } 
      case 'FormB': { 
        self._router.navigate([_routes.clinical.forms.viewFormB(model.id)]);
         break; 
      } 
      case 'FormC': { 
        self._router.navigate([_routes.clinical.forms.viewFormC(model.id)]);
         break; 
      } 
      case 'FormADR': { 
        self._router.navigate([_routes.clinical.forms.viewFormADR(model.id)]);
         break; 
      } 
   }     
  }

  viewMessagesPopUp(data: string[]) {
    let self = this;
    let title = 'Synchronisation messages';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ViewErrorPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, messages: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        return;
      })
  }

  synchSelectedForms(): void {
    const self = this;
    self.setBusy(true);

    self.viewModel.mainGrid.records.data
      .filter(form => form.selected)
      .forEach((form, i)  => {
        setTimeout(() => {
          self.submitForm(form);
        }, i * 1000);
      });
    self.setBusy(false);
  }

  submitForm(record: GridRecordModel): void {
    const self = this;

    self.metaFormService.getForm(record.id).then(result => {
        let form = result as Form;
        self.CLog(form, 'form for submission');

        record.submissionStatus = "InProgress";
        
        var firstForm = JSON.parse(form.formValues[1].formControlValue);

        var medications: PatientMedicationForUpdateModel[] = JSON.parse(form.formValues[4].formControlValue);
        from(medications).pipe(
          concatMap(medicationForUpdate => self.patientService.savePatientMedication(firstForm.patientId, medicationForUpdate.id, medicationForUpdate))
        ).pipe(
          finalize(() => self.saveOnlineMedicationsComplete(record, form)),
        ).subscribe(
          data => {
            self.CLog('subscription to save meds');
          },
          error => {
            this.handleError(error, "Error saving medications");
          }); 
    }, error => {
          self.throwError(error, error.statusText);
    });
  }

  private saveOnlineMedicationsComplete(record: GridRecordModel, form: Form): void {
    const self = this;
    const requestArray = [];

    var firstForm = JSON.parse(form.formValues[1].formControlValue);

    var thirdForm = JSON.parse(form.formValues[2].formControlValue);
    var fourthForm = JSON.parse(form.formValues[3].formControlValue);
    var sixthForm = JSON.parse(form.formValues[5].formControlValue);

    self.CLog('saving meds complete');
    var clinicalEventForUpdate = self.prepareClinicalEventForUpdateModel(firstForm, thirdForm, fourthForm, sixthForm);
    requestArray.push(this.patientService.savePatientClinicalEvent(firstForm.patientId, 0, clinicalEventForUpdate));

    var attachments: FormAttachmentModel[] = [];
    form.formAttachments.forEach(attachment => {
      attachments.push({ description: attachment.description, file: attachment.file })  
    });
    attachments.forEach(attachmentForUpdate => {
      requestArray.push(this.patientService.saveAttachment(firstForm.patientId, attachmentForUpdate.file, attachmentForUpdate.description));
    });

    forkJoin(requestArray)
    .subscribe(
      data => {
        self.CLog('success', 'Form Submission')
        record.submissionStatus = "Successful";
        record.selected = false;
        self.metaFormService.markFormAsSynched(form.id);        
      },
      error => {
        self.CLog(error, 'error saving form to API')
        record.submissionStatus = "Error";
        let messages = [];
        messages.push(self.extractError(error));
        record.synchMessages = error.message;
      });
  }

  private prepareClinicalEventForUpdateModel(firstForm: any, thirdForm: any, fourthForm: any, sixthForm: any): PatientClinicalEventForUpdateModel {
    let self = this;
    self.CLog(thirdForm['dateOfOnset'], 'onsetDate');
    let onsetDate = '';
    if(moment.isMoment(thirdForm['dateOfOnset'])) {
      self.CLog('ismoment', 'onsetDate');
      onsetDate = thirdForm['dateOfOnset'].format('YYYY-MM-DD');
    }
    else {
      if (thirdForm['dateOfOnset'] != '') {
        onsetDate = thirdForm['dateOfOnset'];
      }
    }
    
    self.CLog('about to submit', '');
    let resolutionDate = '';
    if(moment.isMoment(fourthForm['dateOfRecovery'])) {
      resolutionDate = fourthForm['dateOfRecovery'].format('YYYY-MM-DD');
    }
    else {
      if (fourthForm['dateOfRecovery'] != '') {
        resolutionDate = fourthForm['dateOfRecovery'];
      }
    }

    const attributesForUpdate: AttributeValueForPostModel[] = [];

    attributesForUpdate.push(self.prepareAttributeValue('regimen', 'regimen', thirdForm));
    attributesForUpdate.push(self.prepareAttributeValue('is the adverse event serious?', 'isSerious', thirdForm));
    attributesForUpdate.push(self.prepareAttributeValue('seriousness', 'seriousness', thirdForm));
    attributesForUpdate.push(self.prepareAttributeValue('classification', 'classification', thirdForm));
    attributesForUpdate.push(self.prepareAttributeValue('weight (kg)', 'weight', thirdForm));
    attributesForUpdate.push(self.prepareAttributeValue('height (cm)', 'height', thirdForm));
    attributesForUpdate.push(self.prepareAttributeValue('any known allergy', 'allergy', thirdForm));
    attributesForUpdate.push(self.prepareAttributeValue('pregnancy status', 'pregnancyStatus', thirdForm));
    attributesForUpdate.push(self.prepareAttributeValue('comorbidities', 'comorbidities', thirdForm));
    
    attributesForUpdate.push(self.prepareAttributeValue('was treatment given?', 'treatmentGiven', fourthForm));
    attributesForUpdate.push(self.prepareAttributeValue('treatment details', 'treatmentDetails', fourthForm));
    attributesForUpdate.push(self.prepareAttributeValue('outcome', 'outcome', fourthForm));
    attributesForUpdate.push(self.prepareAttributeValue('sequlae details', 'sequlae', fourthForm));
    attributesForUpdate.push(self.prepareAttributeValue('interventions', 'interventions', fourthForm));

    attributesForUpdate.push(self.prepareAttributeValue('name of reporter', 'reporterName', sixthForm));
    attributesForUpdate.push(self.prepareAttributeValue('contact number', 'contactNumber', sixthForm));
    attributesForUpdate.push(self.prepareAttributeValue('email address', 'emailAddress', sixthForm));
    attributesForUpdate.push(self.prepareAttributeValue('profession', 'profession', sixthForm));
    self.CLog(attributesForUpdate, 'clinical event attributes for submission');

    const clinicalEventForUpdate: PatientClinicalEventForUpdateModel = 
    {
      id: 0,
      index: 1,
      patientIdentifier: firstForm['patientIdentifier'],
      onsetDate,
      sourceDescription: thirdForm['sourceDescription'],
      resolutionDate,
      sourceTerminologyMedDraId: null,
      attributes: attributesForUpdate
    };

    return clinicalEventForUpdate;
  }

  private prepareAttributeValue(attributeKey: string, formKey: string, sourceForm: any): AttributeValueForPostModel {
    const self = this;
    let customAttribute = self.viewModel.customAttributeList.find(ca => ca.attributeKey.toLowerCase() == attributeKey.toLowerCase());
    if(customAttribute == null) {
      return null;
    }

    const attributeForPost: AttributeValueForPostModel = {
      id: customAttribute.id,
      value: sourceForm[formKey]
    }    
    return attributeForPost;
  }

  private extractError(errorObject: any): string {
    let message = "Unknown error experienced. Please contact your system administrator. ";
    if(errorObject.error) {
      if(Array.isArray(errorObject.error.Message)) {
        message = errorObject.error.Message[0];
      }
      else {
        message = errorObject.error.Message;
      }
    }

    if(!errorObject.error && errorObject.message) {
      if(Array.isArray(errorObject.message)) {
        message = errorObject.message[0];
      }
      else {
        message = errorObject.message;
      }
    }

    if(errorObject.ReferenceCode) {
      message += `Reference Code: ${errorObject.ReferenceCode}`;
    }

    return message;
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['selected', 'form-type', 'identifier', 'patient-identifier', 'patient-name', 'synch-status', 'actions']);

  formType: string;
  connected: boolean = true;
  selectToggleFlag: boolean = false;

  customAttributeList: CustomAttributeDetailModel[] = [];
}

class GridRecordModel {
  id: number;
  selected?: any;
  created: string;
  formIdentifier: string;
  patientIdentifier: string;
  patientName: string;
  completeStatus: string;
  formType: string;
  hasAttachment: boolean;
  hasSecondAttachment: boolean;
  submissionStatus?: 'None' | 'Error' | 'Successful' | 'InProgress' = 'None';
  synchMessages?: string[] = [];
}