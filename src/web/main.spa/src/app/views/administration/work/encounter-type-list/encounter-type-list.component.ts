import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { EncounterTypeService } from 'app/shared/services/encounter-type.service';
import { EncounterTypePopupComponent } from './encounter-type-popup/encounter-type.popup.component';
import { EncounterTypeDeletePopupComponent } from './encounter-type-delete-popup/encounter-type-delete.popup.component';

@Component({
  templateUrl: './encounter-type-list.component.html',
  styleUrls: ['./encounter-type-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class EncounterTypeListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected encounterTypeService: EncounterTypeService,
    protected dialog: MatDialog,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);
  }

  formControl: FormControl = new FormControl();
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
  } 

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.encounterTypeService.getEncounterTypesByDetail(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  openPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Encounter Type' : 'Update Encounter Type';
    let dialogRef: MatDialogRef<any> = self.dialog.open(EncounterTypePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { encounterTypeId: isNew ? 0: data.id, title: title, payload: data }
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
    let title = 'Delete Encounter Type';
    let dialogRef: MatDialogRef<any> = self.dialog.open(EncounterTypeDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { encounterTypeId: data.id, title: title, payload: data }
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
          (['id', 'type', 'help', 'work-plan', 'actions']);
}

class GridRecordModel {
  id: number;
  encounterTypeName: string;
  help: string;
  workPlanName: string;
}