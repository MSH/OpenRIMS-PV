import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Subscription } from 'rxjs';
import { GridModel } from 'app/shared/models/grid.model';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { CohortPopupComponent } from './cohort-popup/cohort.popup.component';
import { CohortDeletePopupComponent } from './cohort-delete-popup/cohort-delete.popup.component';
import { _routes } from 'app/config/routes';

@Component({
  templateUrl: './cohort-search.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-actions { flex: 0 0 15% !important; width: 15% !important; }
  `],  
  animations: egretAnimations
})
export class CohortSearchComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected eventService: EventService,
    protected cohortGroupService: CohortGroupService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog,
    public accountService: AccountService
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

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
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
    this.eventService.removeAll(CohortSearchComponent.name);
  } 

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.cohortGroupService.getCohortGroupsByDetail(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.mainGrid.updateAdvance(result);
      }, error => {
        self.handleError(error, "Error fetching cohorts");
      });
  }   

  detail(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.clinical.cohorts.view(model.id)]);
  }

  openPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Cohort' : 'Update Cohort';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CohortPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { cohortGroupId: isNew ? 0: data.id, title: title, payload: data }
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

  openDeletePopUp(data: any = {}) {
    let self = this;
    let title = 'Delete Cohort';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CohortDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { cohortGroupId: data.id, title: title, payload: data }
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
          (['cohort-id', 'cohort-name', 'cohort-code', 'primary-condition', 'patient-count', 'start-date', 'finish-date', 'actions']);
}

class GridRecordModel {
  id: number;
  cohortName: string;
  cohortCode: string;
  conditionName: string;
  enrolmentCount: number;
  patientCount: number;
  startDate: string;
  finishDate: string;
}
