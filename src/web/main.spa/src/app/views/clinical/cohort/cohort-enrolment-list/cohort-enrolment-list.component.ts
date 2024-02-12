import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Subscription } from 'rxjs';
import { GridModel } from 'app/shared/models/grid.model';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { _routes } from 'app/config/routes';

@Component({
  templateUrl: './cohort-enrolment-list.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-actions { flex: 0 0 15% !important; width: 15% !important; }
  `],  
  animations: egretAnimations
})
export class CohortEnrolmentListComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected cohortGroupService: CohortGroupService,
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

  id: number;
  viewModel: ViewModel = new ViewModel();
  viewGridModel: ViewGridModel = new ViewGridModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.id = +self._activatedRoute.snapshot.paramMap.get('id');

    self.viewModelForm = self._formBuilder.group({
      cohortName: [this.viewModel.cohortName],
      cohortCode: [this.viewModel.cohortCode],
      conditionName: [this.viewModel.conditionName],
      enrolmentCount: [this.viewModel.enrolmentCount],
      nonSeriousEventCount: [this.viewModel.nonSeriousEventCount],
      seriousEventCount: [this.viewModel.seriousEventCount]
    });  
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewGridModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadData();
    self.loadGrid();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(CohortEnrolmentListComponent.name);
  } 

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.cohortGroupService.getCohortGroupDetail(self.id)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, (self.viewModel = result));
      }, error => {
        this.handleError(error, "Error fetching cohort group");
      });
  }
  
  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.cohortGroupService.getCohortGroupEnrolmentsByDetail(self.id, self.viewGridModel.mainGrid.customFilterModel(self.viewModelForm.value))
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewGridModel.mainGrid.updateAdvance(result);

        const nonSeriousCountSum = result.value.reduce((nonSeriousCountSum, current) => nonSeriousCountSum + current.nonSeriousEventCount, 0);
        self.updateForm(self.viewModelForm, {nonSeriousEventCount: nonSeriousCountSum});

        const seriousCountSum = result.value.reduce((seriousCountSum, current) => seriousCountSum + current.seriousEventCount, 0);
        self.updateForm(self.viewModelForm, {seriousEventCount: seriousCountSum});
      }, error => {
        this.handleError(error, "Error fetching cohort group enrolments");
      });
  }   

  detail(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.clinical.patients.view(model.patientId)]);
  }

  navigateToCohortSearch(): void {
    let self = this;
    self._router.navigate([_routes.clinical.cohorts.search]);
  }
}

class ViewModel {
  cohortName: string;
  cohortCode: string;
  conditionName: string;
  enrolmentCount: number;
  nonSeriousEventCount: number;
  seriousEventCount: number;
}

class ViewGridModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['patient-name', 'facility-name', 'date-of-birth', 'last-encounter', 'current-weight', 'non-serious-events', 'serious-events', 'actions']);
}

class GridRecordModel {
  id: number;
  patientId: number;
  cohortGroupId: number;
  enroledDate: any;
  deenroledDate: any;
  fullName: string;
  facilityName: string;
  dateOfBirth: any;
  age: number;
  latestEncounterDate: any;
  currentWeight?: number;
  nonSeriousEventCount: number;
  seriousEventCount: number;
}
