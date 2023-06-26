import { Component, OnInit, ViewChild, OnDestroy, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
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
import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { PatientService } from 'app/shared/services/patient.service';
import { ConfigService } from 'app/shared/services/config.service';

const moment =  _moment;

@Component({
  templateUrl: './adverse-event-frequency.component.html',
  styles: [`
    .mat-column-system-organ-class { flex: 0 0 20% !important; width: 20% !important; }
  `],  
  animations: egretAnimations
})
export class AdverseEventFrequencyComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected eventService: EventService,
    protected configService: ConfigService, 
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

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  metaDate: string = '';

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
      criteriaId: [this.viewModel.criteriaId || '1', Validators.required],
      searchFrom: [this.viewModel.searchFrom || moment().subtract(3, 'months'), Validators.required],
      searchTo: [this.viewModel.searchTo || moment(), Validators.required],
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadMetaDate();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(AdverseEventFrequencyComponent.name);
  }  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.patientService.getAdverseEventFrequencyReport(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
    .pipe(takeUntil(self._unsubscribeAll))
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.viewModel.mainGrid.updateAdvance(result);
    }, error => {
      self.handleError(error, "Error fetching adverse event frequency report");
    });
  }  

  loadMetaDate(): void {
    let self = this;
    self.configService.getConfigIdentifier(2)
      .subscribe(result => {
        self.metaDate = result.configValue
      });
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['system-organ-class', 'period', 'grade-1', 'grade-2', 'grade-3', 'grade-4', 'grade-5', 'grade-unknown']);

  criteriaId: number;
  stratifyId: number;
  searchFrom: Moment;
  searchTo: Moment;
}

class GridRecordModel {
  systemOrganClass: string;
  periodDisplay: string;
  grade1Count: number;
  grade2Count: number;
  grade3Count: number;
  grade4Count: number;
  grade5Count: number;
  gradeUnknownCount: number;
}