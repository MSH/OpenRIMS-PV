import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { FormGroup, FormBuilder } from '@angular/forms';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { PatientService } from 'app/shared/services/patient.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AppointmentPopupComponent } from './appointment-popup/appointment.popup.component';
import { _routes } from 'app/config/routes';
import { EnrolmentPopupComponent } from './enrolment-popup/enrolment.popup.component';
import { DeenrolmentPopupComponent } from './deenrolment-popup/deenrolment.popup.component';
import { PatientDetailModel } from 'app/shared/models/patient/patient.detail.model';
import { ProgressStatus, ProgressStatusEnum } from 'app/shared/models/program-status.model';
import { HttpEventType } from '@angular/common/http';
import { PatientUpdatePopupComponent } from './patient-update-popup/patient-update.popup.component';
import { AttachmentPopupComponent } from './attachment-popup/attachment.popup.component';
import { EncounterPopupComponent } from './encounter-popup/encounter.popup.component';
import { ConditionPopupComponent } from '../../shared/condition-popup/condition.popup.component';
import { ClinicalEventPopupComponent } from '../../shared/clinical-event-popup/clinical-event.popup.component';
import { GenericDeletePopupComponent } from '../../shared/generic-delete-popup/generic-delete.popup.component';
import { ConditionViewPopupComponent } from '../../shared/condition-view-popup/condition-view.popup.component';
import { GenericArchivePopupComponent } from '../../shared/generic-archive-popup/generic-archive.popup.component';
import { MedicationPopupComponent } from '../../shared/medication-popup/medication.popup.component';
import { LabTestPopupComponent } from '../../shared/lab-test-popup/lab-test.popup.component';
import { EnrolmentIdentifierModel } from 'app/shared/models/cohort/enrolment.identifier.model';
import { ClinicalEventViewPopupComponent } from '../../shared/clinical-event-view-popup/clinical-event-view.popup.component';

@Component({
  templateUrl: './patient-view.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-case-number { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-encounter-date { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-effective-date { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-start-date { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-end-date { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-onset-date { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-test-date { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-outcome-date { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-outcome { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-reported-date { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-resolution-date { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-is-serious { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-enroled-date { flex: 0 0 15% !important; width: 20% !important; }
    .mat-column-de-enroled-date { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-dose { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-dose-unit { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-dose-frequency { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-test-result-coded { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-test-result-value { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-test-unit { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-clinical-event { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }  
  `],  
  animations: egretAnimations
})
export class PatientViewComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected patientService: PatientService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          //this.setupTable();
      }
    });     
  }

  selectedOption = 1;

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  percentage: number;
  showProgress: boolean;

  id: number;
  viewModel: ViewModel;
  viewGridModel: ViewGridModel;
  
  viewBasicForm: FormGroup;
  viewNotesForm: FormGroup;
  viewAuditForm: FormGroup;
    
  ngOnInit(): void {
    const self = this;

    self.id = +self._activatedRoute.snapshot.paramMap.get('id');

    self.viewModel = new ViewModel();
    self.viewGridModel = new ViewGridModel();

    self.viewBasicForm = self._formBuilder.group({
      id: [this.viewModel.id],
      firstName: [this.viewModel.firstName],
      lastName: [this.viewModel.lastName],
      middleName: [this.viewModel.middleName],
      dateOfBirth: [this.viewModel.dateOfBirth],
      facilityName: [this.viewModel.facilityName],
      age: [this.viewModel.age],
      ageGroup: [this.viewModel.ageGroup],
      createdDetail: [this.viewModel.createdDetail],
    });

    self.viewNotesForm = self._formBuilder.group({
      notes: [this.viewModel.notes],
    });

    self.viewAuditForm = self._formBuilder.group({
      id: [this.viewModel.id],
      patientGuid: [this.viewModel.patientGuid],
      createdDetail: [this.viewModel.createdDetail],
      updatedDetail: [this.viewModel.updatedDetail],
    });    
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.id > 0) {
        self.viewGridModel.customGrid.setupBasic(null, null, null);
        self.viewGridModel.appointmentGrid.setupBasic(null, null, null);
        self.viewGridModel.attachmentGrid.setupBasic(null, null, null);
        self.viewGridModel.statusGrid.setupBasic(null, null, null);
        self.viewGridModel.encounterGrid.setupBasic(null, null, null);
        self.viewGridModel.cohortGrid.setupBasic(null, null, null);
        self.viewGridModel.conditionGroupGrid.setupBasic(null, null, null);
        self.viewGridModel.analyticalGrid.setupBasic(null, null, null);

        self.viewGridModel.conditionGrid.setupBasic(null, null, null);
        self.viewGridModel.clinicalEventGrid.setupBasic(null, null, null);
        self.viewGridModel.medicationGrid.setupBasic(null, null, null);
        self.viewGridModel.labTestGrid.setupBasic(null, null, null);

        self.loadData();
    }
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(PatientViewComponent.name);
  }  

  selectAction(action: number): void {
    this.selectedOption = action;
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.patientService.getPatientExpanded(self.id)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewBasicForm, (self.viewModel = result));
        self.updateForm(self.viewNotesForm, (self.viewModel = result));
        self.updateForm(self.viewAuditForm, (self.viewModel = result));
        
        self.viewGridModel.customGrid.updateBasic(result.patientAttributes);
        self.viewGridModel.appointmentGrid.updateBasic(result.appointments);
        self.viewGridModel.attachmentGrid.updateBasic(result.attachments);
        self.viewGridModel.statusGrid.updateBasic(result.patientStatusHistories);
        self.viewGridModel.encounterGrid.updateBasic(result.encounters);
        self.viewGridModel.cohortGrid.updateBasic(result.cohortGroups);
        self.viewGridModel.conditionGroupGrid.updateBasic(result.conditionGroups);
        self.viewGridModel.analyticalGrid.updateBasic(result.activity);

        self.viewGridModel.conditionGrid.updateBasic(result.patientConditions);
        self.viewGridModel.clinicalEventGrid.updateBasic(result.patientClinicalEvents);
        self.viewGridModel.medicationGrid.updateBasic(result.patientMedications);
        self.viewGridModel.labTestGrid.updateBasic(result.patientLabTests);

      }, error => {
        this.handleError(error, "Error loading patient");
      });
  }

  openAppointmentPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Appointment' : 'Update Appointment';
    let dialogRef: MatDialogRef<any> = self.dialog.open(AppointmentPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { patientId: self.id, appointmentId: isNew ? 0: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openAttachmentPopUp() {
    let self = this;
    let title = 'Add Attachment';
    let dialogRef: MatDialogRef<any> = self.dialog.open(AttachmentPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { patientId: self.id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openEncounterPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = 'Add Encounter';
    let dialogRef: MatDialogRef<any> = self.dialog.open(EncounterPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { patientId: self.id, encounterId: isNew ? 0: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openEnrolmentPopUp(cohortGroupId:number, cohortName: string, cohortCode: string, conditionStartDate:string, data: any = {}) {
    let self = this;
    let title = 'Cohort Enrolment';

    let dialogRef: MatDialogRef<any> = self.dialog.open(EnrolmentPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { patientId: self.id, cohortGroupId: cohortGroupId, cohort: `${cohortName} (${cohortCode})`, conditionStartDate: conditionStartDate, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();        
      })
  }

  openConditionPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Condition' : 'Update Condition';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ConditionPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.id, conditionId: isNew ? 0: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openClinicalEventPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Adverse Event' : 'Update Adverse Event';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ClinicalEventPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.id, clinicalEventId: isNew ? 0: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openMedicationPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Medication' : 'Update Medication';
    let dialogRef: MatDialogRef<any> = self.dialog.open(MedicationPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.id, medicationId: isNew ? 0: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openLabTestPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Test and Procedure' : 'Update Test and Procedure';
    let dialogRef: MatDialogRef<any> = self.dialog.open(LabTestPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.id, labTestId: isNew ? 0: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }

  openConditionViewPopUp(patientConditionId: number) {
    let self = this;
    let title = 'View Condition';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ConditionViewPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.id, conditionId: patientConditionId, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        return;
      })
  }

  openClinicalEventViewPopUp(patientClinicalEventId: number) {
    let self = this;
    let title = 'View Adverse Event';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ClinicalEventViewPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.id, clinicalEventId: patientClinicalEventId, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        return;
      })
  }

  openDeletePopUp(type: string, id: number, name: string) {
    let self = this;
    let title = 'Delete ' + type;
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { id: id, patientId: self.id, type: type, title: title, name: name }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();
      })
  }  

  openArchivePopUp(type: string, id: number, name: string, route: boolean = false) {
    let self = this;
    let title = 'Delete ' + type;
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericArchivePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { id: id, parentId: self.id, type: type, title: title, name: name }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        if(route) {
          self._router.navigate([_routes.clinical.patients.search]);
        }
        self.loadData();
      })
  }  

  openDeenrolmentPopUp(cohortGroupEnrolmentId:number, cohortName: string, cohortCode: string, enroledDate:string, data: any = {}) {
    let self = this;
    let title = 'Cohort De-enrolment';

    let dialogRef: MatDialogRef<any> = self.dialog.open(DeenrolmentPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.id, cohortGroupEnrolmentId: cohortGroupEnrolmentId, cohort: `${cohortName} (${cohortCode})`, enroledDate: enroledDate, deenroledDate: Date.now, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();        
      })
  }  

  detailEncounter(model: EncounterGridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.clinical.encounters.view(model ? model.patient.id : 0, model ? model.id : 0)]);
  }    

  downloadAttachment(model: AttachmentGridRecordModel = null): void {
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.patientService.downloadAttachment(this.id, model.id).subscribe(
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

  downloadAllAttachment(): void {
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.patientService.downloadAllAttachment(this.id).subscribe(
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

  openPatientPopUp() {
    let self = this;
    let title = 'Update Patient';
    let dialogRef: MatDialogRef<any> = self.dialog.open(PatientUpdatePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadData();        
      })  
  }
}

class ViewModel {
  id: number;
  patientGuid: string;
  facilityName: string;
  firstName: string;
  middleName: string;
  lastName: string;
  dateOfBirth: any;
  age: number;
  ageGroup: string;
  createdDetail: string;
  updatedDetail: string;
  latestEncounterDate: any;
  medicalRecordNumber: string;
  notes: string;
}

class ViewGridModel {
  customGrid: GridModel<CustomGridRecordModel> =
    new GridModel<CustomGridRecordModel>
        (['key', 'value']);

  appointmentGrid: GridModel<AppointmentGridRecordModel> =
    new GridModel<AppointmentGridRecordModel>
        (['date', 'reason', 'outcome', 'actions']);
    
  attachmentGrid: GridModel<AttachmentGridRecordModel> =
    new GridModel<AttachmentGridRecordModel>
        (['type', 'name', 'description', 'created-by', 'actions']);

  encounterGrid: GridModel<EncounterGridRecordModel> =
  new GridModel<EncounterGridRecordModel>
      (['encounter-date', 'type', 'actions']);
    
  statusGrid: GridModel<StatusGridRecordModel> =
  new GridModel<StatusGridRecordModel>
      (['effective-date', 'status', 'created-by']);
    
  cohortGrid: GridModel<CohortGridRecordModel> =
  new GridModel<CohortGridRecordModel>
      (['cohort', 'start-date', 'enroled-date', 'de-enroled-date', 'actions']);

  conditionGrid: GridModel<ConditionGridRecordModel> =
  new GridModel<ConditionGridRecordModel>
      (['condition-name', 'case-number', 'start-date', 'outcome-date', 'outcome', 'actions']);

  clinicalEventGrid: GridModel<ClinicalEventGridRecordModel> =
  new GridModel<ClinicalEventGridRecordModel>
      (['description', 'onset-date', 'reported-date', 'resolution-date', 'is-serious', 'actions']);

  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['drug-name', 'dose', 'dose-unit', 'dose-frequency', 'start-date', 'end-date', 'indication-type', 'actions']);
          
  labTestGrid: GridModel<LabTestGridRecordModel> =
  new GridModel<LabTestGridRecordModel>
      (['test', 'test-date', 'test-result-coded', 'test-result-value', 'test-unit', 'actions']);

  conditionGroupGrid: GridModel<ConditionGroupGridRecordModel> =
  new GridModel<ConditionGroupGridRecordModel>
      (['condition-status', 'detail', 'actions']);

  analyticalGrid: GridModel<AnalyticalGridRecordModel> =
  new GridModel<AnalyticalGridRecordModel>
      (['clinical-event', 'detail', 'actions']);
}

class CustomGridRecordModel {
  key: number;
  value: string;
  selectionValue: string;
}

class AppointmentGridRecordModel {
  id: number;
  appointmentDate: string;
  reason: string;
  outcome: string;
}

class AttachmentGridRecordModel {
  id: number;
  fileName: string;
  description: string;
  attachmentTyoe: string;
  createdDetail: string;
  updatedDetail: string;
}

class EncounterGridRecordModel {
  id: number;
  patient: PatientDetailModel;
  encounterDate : string;
  encounterType: string;
}

class StatusGridRecordModel {
  effectiveDate: string;
  patientStatus: string;
  createdDetail: string;
}

class CohortGridRecordModel {
  id: number;
  cohortName: string;
  cohortCode: string;
  startDate: string;
  conditionStartDate: string;
  cohortGroupEnrolment: EnrolmentIdentifierModel;
}

class ConditionGridRecordModel {
  id: number;
  sourceDescription: string;
  startDate: string;
  outcomeDate: string;
  outcome: string;
}

class ClinicalEventGridRecordModel {
  id: number;
  sourceDescription: string;
  onsetDate: string;
  reportDate: string;
  resolutionDate: string;
  isSerious: string;
}

class MedicationGridRecordModel {
  id: number;
  sourceDescription: string;
  dose: string;
  doseUnit: string;
  doseFrequency: string;
  startDate: string;
  endDate: string;
  indicationType: string;
}

class LabTestGridRecordModel {
  id: number;
  labTest: string;
  testDate: string;
  testResultCoded: string;
  testResultValue: string;
  testUnit: string;
  rangeLimit: string;
}

class ConditionGroupGridRecordModel {
  conditionGroup: string;
  status: string;
  detail: string;
  patientConditionId: number;
}

class AnalyticalGridRecordModel {
  adverseEvent?: string;
  patientClinicalEventId?: number;
  activity: string;
  executionEvent: string;
  executedDate: string;
  comments: string;
}