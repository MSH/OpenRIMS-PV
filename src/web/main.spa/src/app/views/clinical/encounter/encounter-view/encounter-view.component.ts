import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { GridModel } from 'app/shared/models/grid.model';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { EncounterService } from 'app/shared/services/encounter.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AppLoaderService } from 'app/shared/services/app-loader/app-loader.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { PatientDetailModel } from 'app/shared/models/patient/patient.detail.model';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { DatasetCategoryViewModel } from 'app/shared/models/dataset/dataset-category-view.model';
import { SeriesValueListModel } from 'app/shared/models/dataset/series-value-list.model';
import { _routes } from 'app/config/routes';
import { EncounterUpdatePopupComponent } from './encounter-update-popup/encounter-update.popup.component';
import { EncounterDeletePopupComponent } from './encounter-delete-popup/encounter-delete.popup.component';
import { ConditionPopupComponent } from '../../shared/condition-popup/condition.popup.component';
import { GenericArchivePopupComponent } from '../../shared/generic-archive-popup/generic-archive.popup.component';
import { ConditionViewPopupComponent } from '../../shared/condition-view-popup/condition-view.popup.component';
import { ClinicalEventPopupComponent } from '../../shared/clinical-event-popup/clinical-event.popup.component';
import { MedicationPopupComponent } from '../../shared/medication-popup/medication.popup.component';
import { LabTestPopupComponent } from '../../shared/lab-test-popup/lab-test.popup.component';

@Component({
  templateUrl: './encounter-view.component.html',
  styleUrls: ['./encounter-view.component.scss'],
  animations: egretAnimations
})
export class EncounterViewComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected encounterService: EncounterService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog,
    protected loader: AppLoaderService    
  ) 
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

  id: number;
  patientId: number;
  
  viewModel: ViewModel;
  viewGridModel: ViewGridModel;

  datasetCategories: DatasetCategoryViewModel[] = [];
  weightSeries: SeriesValueListModel[];

  viewEncounterForm: FormGroup;
  viewNotesForm: FormGroup;
  viewAuditForm: FormGroup;

  // weight chart options
  view: any[] = [800, 500];
  xAxisLabel: string = 'Encounter Date';
  yAxisLabel: string = 'Weight (kg)';
  colorScheme = {
    domain: ['#5AA454', '#E44D25', '#CFC0BB', '#7aa3e5', '#a8385d', '#aae3f5']
  };

  ngOnInit(): void {
    const self = this;

    self.patientId = +self._activatedRoute.snapshot.paramMap.get('patientId');
    self.id = +self._activatedRoute.snapshot.paramMap.get('id');

    self.viewModel = new ViewModel();
    self.viewGridModel = new ViewGridModel();

    self.viewEncounterForm = self._formBuilder.group({
      elements: this._formBuilder.group([])
    });

    self.viewAuditForm = self._formBuilder.group({
      id: [this.viewModel.id],
      encounterGuid: [this.viewModel.encounterGuid],
      createdDetail: [this.viewModel.createdDetail],
      updatedDetail: [this.viewModel.updatedDetail],
    });

    self.viewNotesForm = self._formBuilder.group({
      notes: [this.viewModel.notes],
    });    

    if (self.id > 0) {
      self.viewGridModel.conditionGrid.setupBasic(null, null, null);
      self.viewGridModel.clinicalEventGrid.setupBasic(null, null, null);
      self.viewGridModel.medicationGrid.setupBasic(null, null, null);
      self.viewGridModel.labTestGrid.setupBasic(null, null, null);
      self.viewGridModel.conditionGroupGrid.setupBasic(null, null, null);

      self.loadData();
    }
  }

  ngAfterViewInit(): void {
    let self = this;
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(EncounterViewComponent.name);
  }

  selectAction(action: number): void {
    this.selectedOption = action;
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.encounterService.getEncounterExpanded(self.patientId, self.id)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewNotesForm, (self.viewModel = result));
        self.updateForm(self.viewAuditForm, (self.viewModel = result));
        
        self.viewGridModel.conditionGroupGrid.updateBasic(result.conditionGroups);

        self.viewGridModel.conditionGrid.updateBasic(result.patientConditions);
        self.viewGridModel.clinicalEventGrid.updateBasic(result.patientClinicalEvents);
        self.viewGridModel.medicationGrid.updateBasic(result.patientMedications);
        self.viewGridModel.labTestGrid.updateBasic(result.patientLabTests);

        // handle dynamic data
        self.datasetCategories = result.datasetCategories;
        // Add elements to form group
        let elements = self.viewEncounterForm.get('elements') as FormGroup;
        self.datasetCategories.forEach(category => {
          category.datasetElements.forEach(element => {
            elements.removeControl(element.datasetElementId.toString());
            elements.addControl(element.datasetElementId.toString(), new FormControl(element.datasetElementValue));
          })
        })

        // store weight series for charting
        self.weightSeries = result.weightSeries;
      }, error => {
        this.handleError(error, "Error loading encounter");
      });
  }

  detail(model: ConditionGridRecordModel = null): void {
    let self = this;
    //self._router.navigate([_routes.clinical.patients.view(model ? model.id : 0)]);
  }

  routeToSearch(): void {
    let self = this;
    self._router.navigate([_routes.clinical.encounters.search]);
  }    

  routeToPatient(): void {
    let self = this;
    self._router.navigate([_routes.clinical.patients.view(self.patientId)]);
  }    

  openUpdatePopUp() {
    let self = this;
    let title = 'Update Encounter';
    let dialogRef: MatDialogRef<any> = self.dialog.open(EncounterUpdatePopupComponent, {
      width: '1020px',
      height: '720px',
      disableClose: true,
      data: { patientId: self.patientId, encounterId: self.id, title: title }
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

  openDeletePopUp() {
    let self = this;
    let title = 'Delete Encounter';
    let dialogRef: MatDialogRef<any> = self.dialog.open(EncounterDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { patientId: self.patientId, encounterId: self.id, title: title, payload: self.viewModel }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self._router.navigate([_routes.clinical.patients.view(self.patientId)]);
      })  
  }

  openConditionPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Condition' : 'Update Condition';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ConditionPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.patientId, conditionId: isNew ? 0: data.id, title: title, payload: data }
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
      data: { patientId: self.patientId, conditionId: patientConditionId, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        return;
      })
  }

  openClinicalEventPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Adverse Event' : 'Update Adverse Event';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ClinicalEventPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { patientId: self.patientId, clinicalEventId: isNew ? 0: data.id, title: title, payload: data }
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
      data: { patientId: self.patientId, medicationId: isNew ? 0: data.id, title: title, payload: data }
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
      data: { patientId: self.patientId, labTestId: isNew ? 0: data.id, title: title, payload: data }
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

  openArchivePopUp(type: string, id: number, name: string) {
    let self = this;
    let title = 'Delete ' + type;
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericArchivePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { id: id, parentId: self.patientId, type: type, title: title, name: name }
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
  encounterGuid: string;
  encounterDate: string;
  encounterType: string;
  patient: PatientDetailModel;
  createdDetail: string;
  updatedDetail: string;
  notes: string;
}

class ViewGridModel {

  conditionGrid: GridModel<ConditionGridRecordModel> =
  new GridModel<ConditionGridRecordModel>
      (['condition-name', 'start-date', 'outcome-date', 'outcome', 'actions']);

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