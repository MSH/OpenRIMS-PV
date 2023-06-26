import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild } from '@angular/core';
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
import { _routes } from 'app/config/routes';
import { DatasetCategoryElementDeletePopupComponent } from './dataset-category-element-delete-popup/dataset-category-element-delete.popup.component';
import { DatasetElementSelectPopupComponent } from 'app/views/administration/shared/dataset-element-select-popup/dataset-element-select.popup.component';
import { DatasetCategoryElementPopupComponent } from './dataset-category-element-popup/dataset-category-element.popup.component';

@Component({
  templateUrl: './dataset-category-element-list.component.html',
  styleUrls: ['./dataset-category-element-list.component.scss'],
  animations: egretAnimations  
})
export class DatasetCategoryElementListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

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
  datasetCategoryId: number;
  viewModel: ViewModel = new ViewModel();;
  viewModelForm: FormGroup;
  newElementForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;
    
  ngOnInit(): void {
    const self = this;

    self.datasetid = +self._activatedRoute.snapshot.paramMap.get('datasetid');
    self.datasetCategoryId = +self._activatedRoute.snapshot.paramMap.get('datasetcategoryid');

    self.viewModelForm = self._formBuilder.group({
    });

    self.newElementForm = self._formBuilder.group({
      datasetElementId: [''],
      friendlyName: [''],
      help: [''],
      acute: ['No'],
      chronic: ['No']
    })
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
    self.datasetService.getDatasetCategory(self.datasetid, self.datasetCategoryId)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.datasetCategoryName = result.datasetCategoryName;
      }, error => {
        this.handleError(error, "Error fetching dataset category");
      });
  }

  loadGrid(): void {
    let self = this;
    self.setBusy(true);
    self.datasetService.getDatasetCategoryElements(self.datasetid, self.datasetCategoryId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching dataset category elements");
        });
  }

  openPopUp(data: any = {}) {
    let self = this;
    let title = 'Update Element';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetCategoryElementPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { datasetId: self.datasetid, datasetCategoryId: self.datasetCategoryId, datasetCategoryElementId: data.id, title: title, payload: data }
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
    let title = 'Delete Element';
    console.log(data);
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetCategoryElementDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { datasetId: self.datasetid, datasetCategoryId: self.datasetCategoryId, datasetCategoryElementId: data.id, categoryName: self.viewModel.datasetCategoryName, title: title, payload: data }
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

  routeToCategories(): void {
    let self = this;
    self._router.navigate([_routes.administration.work.datasetcategoryView(self.datasetid)]);
  }    

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(DatasetCategoryElementListComponent.name);
  }

  openDatasetElementSelectPopup() {
    let self = this;
    let title = 'Select Element';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetElementSelectPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        self.updateForm(self.newElementForm, {datasetElementId: result.id});
        self.saveNewElement(result);
      })
  }

  saveNewElement(result: any) {
    let self = this;
    self.setBusy(true);

    self.datasetService.saveDatasetCategoryElement(self.datasetid, self.datasetCategoryId, 0, self.newElementForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset category element saved successfully", "Datasets");
        self.loadGrid();        
    }, error => {
        self.handleError(error, "Error saving new dataset category element");
    });
  }
}

class ViewModel {
  datasetCategoryName: string;

  mainGrid: GridModel<GridRecordModel> =
    new GridModel<GridRecordModel>
        (['element', 'friendly-name', 'help', 'actions']);
}

class GridRecordModel {
  id: number;
  elementName: string;
  datasetElementId: number;
  fieldOrder: number;
  system: string;
  acute: string;
  chronic: string;
  friendlyName: string;
  help: string;
}
