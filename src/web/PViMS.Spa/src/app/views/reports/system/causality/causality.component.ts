import { Component, OnInit, ViewChild, OnDestroy, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from 'app/shared/services/event.service';
import { FacilityService } from 'app/shared/services/facility.service';
import { AccountService } from 'app/shared/services/account.service';
import { PopupService } from 'app/shared/services/popup.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { BaseComponent } from 'app/shared/base/base.component';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { _routes } from 'app/config/routes';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { FacilityIdentifierModel } from 'app/shared/models/facility/facility.identifier.model';
import { ConfigService } from 'app/shared/services/config.service';

const moment =  _moment;

@Component({
  templateUrl: './causality.component.html',
  animations: egretAnimations
})
export class CausalityComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected facilityService: FacilityService,
    protected reportInstanceService: ReportInstanceService,
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

  facilityList: FacilityIdentifierModel[] = [];

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.viewModelForm = self._formBuilder.group({
      facilityId: [this.viewModel.facilityId || '0', Validators.required],
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
    this.eventService.removeAll(CausalityComponent.name);
  }  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.reportInstanceService.getCausalityReport(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          self.handleError(error, "Error fetching causality report");
        });
  }  

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
  }

  getFacilityList(): void {
    let self = this;
    self.facilityService.getAllFacilities()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
          self.facilityList = result;
        }, error => {
          self.handleError(error, "Error fetching facility list");
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
          (['first-name', 'last-name', 'facility', 'adverse-event', 'serious', 'onset-date', 'medication', 'who-causality', 'naranjo-causality']);

  facilityId: number;
  criteriaId: number;
  searchFrom: Moment;
  searchTo: Moment;
}

class GridRecordModel {
  id: number;
  firstName: string;
  lastName: string;
  facilityName: string;
  adverseEvent: string;
  serious: string;
  onsetDate: string;
  MedicationIdentifier: string;
  whoCausality: string;
  naranjoCausality: string;
}