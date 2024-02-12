import { Component, OnInit, ViewChild, OnDestroy, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from 'app/shared/services/event.service';
import { AccountService } from 'app/shared/services/account.service';
import { PopupService } from 'app/shared/services/popup.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { BaseComponent } from 'app/shared/base/base.component';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { _routes } from 'app/config/routes';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { PatientService } from 'app/shared/services/patient.service';
import { PatientListModel } from 'app/shared/models/patient/patient-list.model';
import { PatientListPopupComponent } from '../../shared/patient-list/patient-list.popup.component';
import { ConfigService } from 'app/shared/services/config.service';


@Component({
  templateUrl: './patient-medication.component.html',
  styleUrls: ['./patient-medication.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class PatientMedicationComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected patientService: PatientService,
    protected configService: ConfigService,
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

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  metaDate: string = '';

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
      conceptName: [this.viewModel.conceptName || ''],
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
    self.loadMetaDate();    
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(PatientMedicationComponent.name);
  }  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.patientService.getPatientMedicationReport(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching report");
        });
  }

  openPatientPopUp(data: any = {}) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(PatientListPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { patients: data.patients }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
      })
  }

  loadMetaDate(): void {
    let self = this;
    self.configService.getConfigIdentifier(7)
      .subscribe(result => {
        self.metaDate = result.configValue
      });
  }   
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['concept', 'patient-count', 'actions']);

  conceptName: string;
}

class GridRecordModel {
  conceptId: number;
  conceptName: string;
  patientCount: number;
  patients: PatientListModel[];
}