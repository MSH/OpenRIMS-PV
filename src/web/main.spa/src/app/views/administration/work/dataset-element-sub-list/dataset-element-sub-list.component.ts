import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
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
import { DatasetElementService } from 'app/shared/services/dataset-element.service';
import { DatasetElementSubDeletePopupComponent } from './dataset-element-sub-delete-popup/dataset-element-sub-delete.popup.component';
import { DatasetElementSubPopupComponent } from './dataset-element-sub-popup/dataset-element-sub.popup.component';

@Component({
  templateUrl: './dataset-element-sub-list.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }  
  `],  
  animations: egretAnimations
})
export class DatasetElementSubListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected dialog: MatDialog,
    protected datasetElementService: DatasetElementService,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);
  }

  id: number;

  formControl: FormControl = new FormControl();
  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    self.id = +self._activatedRoute.snapshot.paramMap.get('datasetelementid');

    self.viewModelForm = self._formBuilder.group({
      elementName: [this.viewModel.elementName || ''],
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

    self.datasetElementService.getDatasetElementSubs(self.id, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching dataset element subs");
        });
  }

  openPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Element Sub' : 'Update Element Sub';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetElementSubPopupComponent, {
      width: '920px',
      minHeight: '530px',
      disableClose: true,
      data: { datasetElementId: self.id, datasetElementSubId: isNew ? 0: data.id, title: title, payload: data }
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
    let title = 'Delete Element Sub';
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetElementSubDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { datasetElementId: self.id, datasetElementSubId: data.id, title: title, payload: data }
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
          (['id', 'element', 'field-type', 'friendly-name', 'actions']);
  
  elementName: string;
}

class GridRecordModel {
  id: number;
  elementName: string;
  fieldTypeName: string;
}