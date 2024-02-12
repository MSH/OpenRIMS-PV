import { Component, OnInit, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { ConceptService } from 'app/shared/services/concept.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { GridModel } from 'app/shared/models/grid.model';
import { ConceptPopupComponent } from './concept-popup/concept.popup.component';
import { GenericDeletePopupComponent } from '../../shared/generic-delete-popup/generic-delete.popup.component';

@Component({
  templateUrl: './concept-list.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 10% !important; width: 10% !important; }
  `],  
  animations: egretAnimations
})
export class ConceptListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected conceptService: ConceptService,
    protected dialog: MatDialog,    
    protected mediaObserver: MediaObserver) 
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

    self.viewModelForm = self._formBuilder.group({
      searchTerm: [this.viewModel.searchTerm || ''],
      active: ['Both']
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

  setupTable() {
  };

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.conceptService.searchConceptsByDetail(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error searching for concepts");
        });
  }

  openPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Active Ingredient' : 'Update Active Ingredient';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ConceptPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { conceptId: isNew ? 0: data.id, title: title, payload: data }
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

  openDeletePopUp(type: string, id: number, name: string) {
    let self = this;
    let title = 'Delete Active Ingredient';
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { id: id, type: type, title: title, name: name }
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
          (['id', 'active-ingredient', 'strength', 'form', 'active', 'actions']);

  searchTerm: string;
}

class GridRecordModel {
  id: number;
  conceptName: string;
  strength: string;
  formName: string;
  active: string;
}
