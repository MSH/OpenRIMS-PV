import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { GridModel } from 'app/shared/models/grid.model';
import { EncounterService } from 'app/shared/services/encounter.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { PatientDetailModel } from 'app/shared/models/patient/patient.detail.model';
import { _routes } from 'app/config/routes';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { CustomAttributeIdentifierModel } from 'app/shared/models/custom-attribute/custom-attribute.identifier.model';
import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { AppointmentService } from 'app/shared/services/appointment.service';
import { DnaPopupComponent } from '../../shared/dna-popup/dna.popup.component';

const moment =  _moment;

@Component({
  templateUrl: './encounter-search.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }  
  `],   
  animations: egretAnimations
})
export class EncounterSearchComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected encounterService: EncounterService,
    protected appointmentService: AppointmentService,
    protected customAttributeService: CustomAttributeService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          this.setupTable();
      }
    });    
  }

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  formControl: FormControl = new FormControl();
  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  facilityList: string[] = [];
  customAttributeList: CustomAttributeIdentifierModel[] = [];

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.viewModelForm = self._formBuilder.group({
      facilityName: [this.viewModel.facilityName || ''],
      patientId: [this.viewModel.patientId],
      firstName: [this.viewModel.firstName, [Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]],
      lastName: [this.viewModel.lastName, [Validators.maxLength(30), Validators.pattern("[-a-zA-Z ']*")]], 
      criteriaId: [this.viewModel.criteriaId || '1'],
      searchFrom: [this.viewModel.searchFrom],
      searchTo: [this.viewModel.searchTo],
      customAttributeId: [this.viewModel.customAttributeId],
      customAttributeValue: [this.viewModel.customAttributeValue, [Validators.maxLength(150), Validators.pattern("[-a-zA-Z0-9 ']*")]]
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(EncounterSearchComponent.name);
  } 

  setupTable() {
    if (this.currentScreenWidth === 'xs') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['last-name', 'encounter-date', 'actions']);
    }
    if (this.currentScreenWidth === 'sm') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['id', 'first-name', 'last-name', 'encounter-date', 'actions']);
    }
  };

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    if(self.viewModelForm.value.criteriaId == '1') { 
      self.encounterService.searchEncounter(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
      }, error => {
        self.handleError(error, "Error fetching encounters");
      });
    }
    else {
      self.appointmentService.searchAppointment(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
          self.viewModel.appointmentGrid.updateAdvance(result);
      }, error => {
        self.handleError(error, "Error fetching appointments");
      });
    }
  }   

  encounterDetail(patientId: number, encounterId: number): void {
    let self = this;
    self._router.navigate([_routes.clinical.encounters.view(patientId, encounterId)]);
  }

  patientDetail(patientId: number): void {
    let self = this;
    self._router.navigate([_routes.clinical.patients.view(patientId)]);
  }

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
    self.getCustomAttributeList();
  }

  getFacilityList(): void {
    let self = this;
    self.facilityList = self.accountService.facilities;
  }

  getCustomAttributeList(): void {
    let self = this;
    self.customAttributeService.getPatientCustomAttributeSearchableList()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.customAttributeList = result.value;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  openDnaPopUp(data: any = {}) {
    let self = this;
    let title = 'Did not arrive';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DnaPopupComponent, {
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
        self.loadGrid();
      })
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['id', 'first-name', 'last-name', 'facility', 'encounter-type', 'encounter-date', 'actions']);

  appointmentGrid: GridModel<AppointmentGridRecordModel> =
  new GridModel<AppointmentGridRecordModel>
      (['id', 'first-name', 'last-name', 'facility', 'appointment-date', 'appointment-status', 'actions']);
    
  facilityName: string;
  patientId: number;
  firstName: string;
  lastName: string;
  criteriaId: number;
  searchFrom: Moment;
  searchTo: Moment;
  customAttributeId: number;
  customAttributeValue: string;
}

class GridRecordModel {
  id: number;
  patient: PatientDetailModel;
  encounterType: string;
  encounterDate: string;
}

class AppointmentGridRecordModel {
  id: number;
  patientId: number;
  encounterId: number;
  appointmentDate: any;
  firstName: string;
  lastName: string;
  currentFacility: string;
  appointmentStatus: string;
}
