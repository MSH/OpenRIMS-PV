import { Component, OnInit, AfterViewInit, OnDestroy, ViewEncapsulation, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Subscription } from 'rxjs';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { GridModel } from 'app/shared/models/grid.model';
import { takeUntil, finalize } from 'rxjs/operators';
import { DatasetService } from 'app/shared/services/dataset.service';
import { DatasetCategoryPopupComponent } from './dataset-category-popup/dataset-category.popup.component';
import { _routes } from 'app/config/routes';
import { DatasetCategoryDeletePopupComponent } from './dataset-category-delete-popup/dataset-category-delete.popup.component';

@Component({
  templateUrl: './dataset-category-list.component.html',
  styleUrls: ['./dataset-category-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations  
})
export class DatasetCategoryListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected mediaObserver: MediaObserver,
    protected datasetService: DatasetService,
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

  datasetid: number;
  viewModel: ViewModel = new ViewModel();;
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;
    
  ngOnInit(): void {
    const self = this;

    self.datasetid = +self._activatedRoute.snapshot.paramMap.get('datasetid');

    self.viewModelForm = self._formBuilder.group({
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadData();
    self.loadGrid();
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.datasetService.getDataset(self.datasetid)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.datasetName = result.datasetName;
      }, error => {
        this.handleError(error, "Error fetching dataset");
      });
  }  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);
    self.datasetService.getDatasetCategories(self.datasetid, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching dataset categories");
        });
  }

  openPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Category' : 'Update Category';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetCategoryPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { datasetId: self.datasetid, datasetCategoryId: isNew ? 0: data.id, title: title, payload: data }
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
    let title = 'Delete Category';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetCategoryDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { datasetId: self.datasetid, datasetCategoryId: data.id, title: title, payload: data }
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

  elements(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.administration.work.datasetcategoryElementView(self.datasetid, model ? model.id : 0)]);
  }

  routeToDatasets(): void {
    let self = this;
    self._router.navigate([_routes.administration.work.dataset]);
  }    

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(DatasetCategoryListComponent.name);
  }
}

class ViewModel {
  datasetName: string;

  mainGrid: GridModel<GridRecordModel> =
    new GridModel<GridRecordModel>
        (['category', 'friendly-name', 'element-count', 'actions']);
}

class GridRecordModel {
  id: number;
  datasetCategoryName: string;
  friendlyName: string;
  help: string;
  acute: string;
  chronic: string;
  categoryOrder: number;
  elementCount: number;
}
