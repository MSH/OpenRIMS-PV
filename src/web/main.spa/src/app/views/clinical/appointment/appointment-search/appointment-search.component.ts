import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Subscription } from 'rxjs';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AppointmentService } from 'app/shared/services/appointment.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { _routes } from 'app/config/routes';
import { DnaPopupComponent } from '../../shared/dna-popup/dna.popup.component';
import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
const moment =  _moment;

@Component({
  templateUrl: './appointment-search.component.html',
  styleUrls: ['./appointment-search.component.scss'],
  animations: egretAnimations
})
export class AppointmentSearchComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected appointmentService: AppointmentService,
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

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    //self.loadDropDowns();

    self.viewModelForm = self._formBuilder.group({
      criteriaId: ['6'],
      searchFrom: [moment().subtract(3, 'months'), Validators.required],
      searchTo: [moment(), Validators.required],
    });
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  } 

  setupTable() {
    if (this.currentScreenWidth === 'xs') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['patient-name', 'actions']);
    }
  };  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.appointmentService.searchAppointment(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
    .pipe(takeUntil(self._unsubscribeAll))
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      console.log(result);
      self.viewModel.mainGrid.updateAdvance(result);
    }, error => {
      self.handleError(error, "Error fetching appointments");
    });
  } 
    
  encounterDetail(patientId: number, encounterId: number): void {
    let self = this;
    self._router.navigate([_routes.clinical.encounters.view(patientId, encounterId)]);
  }

  patientDetail(patientId: number): void {
    let self = this;
    self._router.navigate([_routes.clinical.patients.view(patientId)]);
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
          (['patient-name', 'details', 'appointment-date', 'activity', 'actions']);

  appointmentDate: any;
}

class GridRecordModel {
  id: number;
  patientId: number;
  encounterId: number;
  appointmentDate: any;
  firstName: string;
  lastName: string;
  currentFacility: string;
  reason: string;
  appointmentStatus: string;
}
