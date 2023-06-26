import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { switchMap } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { GridModel } from 'app/shared/models/grid.model';
import { forkJoin, of } from 'rxjs';
import { AttributeValueForPostModel } from 'app/shared/models/custom-attribute/attribute-value-for-post.model';
import { PatientClinicalEventExpandedModel } from 'app/shared/models/patient/patient-clinical-event.expanded.model';
import { PatientMedicationForUpdateModel } from 'app/shared/models/patient/patient-medication-for-update.model';
import { PatientMedicationDetailModel } from 'app/shared/models/patient/patient-medication.detail.model';
import { PatientExpandedModel } from 'app/shared/models/patient/patient.expanded.model';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';
import { ProgressStatus, ProgressStatusEnum } from 'app/shared/models/program-status.model';
import { HttpEventType } from '@angular/common/http';
import { BaseComponent } from 'app/shared/base/base.component';
import { EventService } from 'app/shared/services/event.service';
const moment =  _moment;

@Component({
  templateUrl: './clinical-details.component.html',
  styles: [`
    .mat-column-executed-date { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-activity { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-execution-event { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-comments { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-source { flex: 0 0 18% !important; width: 18% !important; }
    .mat-column-description { flex: 0 0 30% !important; width: 30% !important; }
    .mat-column-task-type { flex: 0 0 25% !important; width: 25% !important; }
    .mat-column-task-status { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-file-name { flex: 0 0 85% !important; width: 85% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],  
  animations: egretAnimations
})
export class ClinicalDetailsComponent extends BaseComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();

  public firstFormGroup: FormGroup;
  public secondFormGroup: FormGroup;
  public thirdFormGroup: FormGroup;
  public fourthFormGroup: FormGroup;
  public fifthFormGroup: FormGroup;
  public sixthFormGroup: FormGroup; 

  percentage: number;
  showProgress: boolean;

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
  ) { 
    super(_router, _location, popupService, accountService, eventService);
  }

  ngOnInit(): void {
    const self = this;

    self.viewModel.patientId = +self._activatedRoute.snapshot.paramMap.get('patientId');
    self.viewModel.clinicalEventId = +self._activatedRoute.snapshot.paramMap.get('clinicalEventId');

    self.firstFormGroup = this._formBuilder.group({
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
      onsetDate: ['', Validators.required],
      regimen: [null, Validators.required],
      sourceDescription: [null, [Validators.required, Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      isSerious: [null],
      seriousness: [null],
      classification: [null, Validators.required],
      weight: [null, [Validators.required, Validators.min(1), Validators.max(159)]],
      height: [null, [Validators.required, Validators.min(1), Validators.max(259)]],
      allergy: ['', [Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
      pregnancyStatus: [null],
      comorbidities: ['', [Validators.maxLength(500), Validators.pattern("[-a-zA-Z0-9()/., ']*")]],
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
      reporterName: ['', [Validators.maxLength(100), Validators.pattern("[-a-zA-Z ']*")]],
      contactNumber: ['', [Validators.maxLength(30), Validators.pattern("[-0-9+']*")]],
      emailAddress: ['', Validators.maxLength(100)],
      profession: [null]
    });

    self.viewModel.attachmentGrid.setupBasic(null, null, null);
    self.viewModel.medicationGrid.setupBasic(null, null, null);

    self.getCustomAttributeList();    
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.viewModel.clinicalEventId > 0) {
      self.loadData();
    }
  } 
  
  setStep(index: number) {
    this.viewModel.step = index;
  }

  nextStep() {
    this.viewModel.step++;
  }

  prevStep() {
    this.viewModel.step--;
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    requestArray.push(self.patientService.getPatientClinicalEventExpanded(self.viewModel.patientId, self.viewModel.clinicalEventId));
    requestArray.push(self.patientService.getPatientExpanded(self.viewModel.patientId));

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.loadGrids(data[1] as PatientExpandedModel);

          self.loadDataForFirstForm(data[1] as PatientExpandedModel);
          self.loadDataForThirdForm(data[0] as PatientClinicalEventExpandedModel);
          self.loadDataForFourthForm(data[0] as PatientClinicalEventExpandedModel);
          self.loadDataForSixthForm(data[0] as PatientClinicalEventExpandedModel);

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });    
  }

  downloadAttachment(model: AttachmentGridRecordModel = null): void {
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.patientService.downloadAttachment(this.viewModel.patientId, model.id).subscribe(
      data => {
        switch (data.type) {
          case HttpEventType.DownloadProgress:
            this.downloadStatus( {status: ProgressStatusEnum.IN_PROGRESS, percentage: Math.round((data.loaded / data.total) * 100)});
            break;

          case HttpEventType.Response:
            this.downloadStatus( {status: ProgressStatusEnum.COMPLETE});
            
            const downloadedFile = new Blob([data.body], { type: data.body.type });
            const a = document.createElement('a');

            a.setAttribute('style', 'display:none;');
            document.body.appendChild(a);
            a.download = '';
            a.href = URL.createObjectURL(downloadedFile);
            a.target = '_blank';
            a.click();
            document.body.removeChild(a);

            this.notify("Attachment downloaded successfully!", "Success");            
            break;
        }
      },
      error => {
        this.downloadStatus( {status: ProgressStatusEnum.ERROR} );
      }
    );
  }

  downloadStatus(event: ProgressStatus) {
    switch (event.status) {
      case ProgressStatusEnum.START:
        this.setBusy(true);
        break;

      case ProgressStatusEnum.IN_PROGRESS:
        this.showProgress = true;
        this.percentage = event.percentage;
        break;

      case ProgressStatusEnum.COMPLETE:
        this.showProgress = false;
        this.setBusy(false);
        break;

      case ProgressStatusEnum.ERROR:
        this.showProgress = false;
        this.setBusy(false);
        this.throwError('Error downloading file. Please try again.', 'Error downloading file. Please try again.');
        break;
    }
  }

  private loadGrids(patientModel: PatientExpandedModel) {
    let self = this;
    self.viewModel.medicationGrid.updateBasic(patientModel.patientMedications);
    self.viewModel.attachmentGrid.updateBasic(patientModel.attachments);    
  }

  private loadDataForFirstForm(patientModel: PatientExpandedModel)
  {
    let self = this;
    self.updateForm(self.firstFormGroup, patientModel);
    self.updateForm(self.firstFormGroup, {patientId: patientModel.id});
    self.updateForm(self.firstFormGroup, {patientFirstName: patientModel.firstName});
    self.updateForm(self.firstFormGroup, {patientLastName: patientModel.lastName});
    self.updateForm(self.firstFormGroup, {gender: self.getValueOrSelectedValueFromAttribute(patientModel.patientAttributes, "Gender")});
    self.updateForm(self.firstFormGroup, {ethnicity: self.getValueOrSelectedValueFromAttribute(patientModel.patientAttributes, "Ethnic Group")});
    self.updateForm(self.firstFormGroup, {facilityRegion: patientModel.organisationUnit});
  }

  private loadDataForThirdForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.thirdFormGroup, { 'onsetDate': clinicalEventModel.onsetDate })
    self.updateForm(self.thirdFormGroup, { 'regimen': self.getValueOrSelectedValueFromAttribute(clinicalEventModel.clinicalEventAttributes, "Regimen") })
    self.updateForm(self.thirdFormGroup, { 'sourceDescription': clinicalEventModel.sourceDescription })
    self.updateForm(self.thirdFormGroup, { 'isSerious': self.getValueOrSelectedValueFromAttribute(clinicalEventModel.clinicalEventAttributes, "Is the adverse event serious?") })
    self.updateForm(self.thirdFormGroup, { 'seriousness': self.getValueOrSelectedValueFromAttribute(clinicalEventModel.clinicalEventAttributes, "Seriousness") })
    self.updateForm(self.thirdFormGroup, { 'classification': self.getValueOrSelectedValueFromAttribute(clinicalEventModel.clinicalEventAttributes, "Classification") })
    self.updateForm(self.thirdFormGroup, { 'weight': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'weight (kg)')?.value })
    self.updateForm(self.thirdFormGroup, { 'height': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'height (cm)')?.value })
    self.updateForm(self.thirdFormGroup, { 'allergy': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'any known allergy')?.value })
    self.updateForm(self.thirdFormGroup, { 'pregnancyStatus': self.getValueOrSelectedValueFromAttribute(clinicalEventModel.clinicalEventAttributes, "Pregnancy status") })
    self.updateForm(self.thirdFormGroup, { 'comorbidities': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'comorbidities')?.value })
  }

  private loadDataForFourthForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.fourthFormGroup, { 'treatmentGiven': self.getValueOrSelectedValueFromAttribute(clinicalEventModel.clinicalEventAttributes, "Was treatment given?") })
    self.updateForm(self.fourthFormGroup, { 'treatmentDetails': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'treatment details')?.value })
    self.updateForm(self.fourthFormGroup, { 'outcome': self.getValueOrSelectedValueFromAttribute(clinicalEventModel.clinicalEventAttributes, "Outcome") })
    self.updateForm(self.fourthFormGroup, { 'dateOfRecovery': clinicalEventModel.resolutionDate })
    self.updateForm(self.fourthFormGroup, { 'sequlae': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'sequlae details')?.value })
    self.updateForm(self.fourthFormGroup, { 'interventions': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'interventions')?.value })
  }

  private loadDataForSixthForm(clinicalEventModel: PatientClinicalEventExpandedModel) {
    let self = this;

    self.updateForm(self.sixthFormGroup, { 'reporterName': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'name of reporter')?.value })
    self.updateForm(self.sixthFormGroup, { 'contactNumber': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'contact number')?.value })
    self.updateForm(self.sixthFormGroup, { 'emailAddress': clinicalEventModel.clinicalEventAttributes.find(pa => pa.key.toLowerCase() == 'email address')?.value })
    self.updateForm(self.sixthFormGroup, { 'profession': self.getValueOrSelectedValueFromAttribute(clinicalEventModel.clinicalEventAttributes, "Profession") })
  }

  private getCustomAttributeList(): void {
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
}

class ViewModel {
  medicationGrid: GridModel<PatientMedicationDetailModel> =
  new GridModel<PatientMedicationDetailModel>
      (['medication', 'start-date', 'dose', 'reason-for-stopping', 'clinical-action', 'result-of-challenge']);
  medications: PatientMedicationForUpdateModel[] = [];

  attachmentGrid: GridModel<AttachmentGridRecordModel> =
    new GridModel<AttachmentGridRecordModel>
        (['type', 'name', 'description', 'actions']);

  workFlowId = '892F3305-7819-4F18-8A87-11CBA3AEE219';
  step = 0;

  patientId: number;
  clinicalEventId: number;

  customAttributeKey = 'Case Number';
  customAttributeList: CustomAttributeDetailModel[] = [];
}

class AttachmentGridRecordModel {
  id: number;
  fileName: string;
  description: string;
  attachmentTyoe: string;
  createdDetail: string;
  updatedDetail: string;
}