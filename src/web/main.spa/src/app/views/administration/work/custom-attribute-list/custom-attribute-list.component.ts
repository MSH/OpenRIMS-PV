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
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeAddPopupComponent } from './custom-attribute-add-popup/custom-attribute-add.popup.component';
import { CustomAttributeDeletePopupComponent } from './custom-attribute-delete-popup/custom-attribute-delete.popup.component';
import { CustomAttributeEditPopupComponent } from './custom-attribute-edit-popup/custom-attribute-edit.popup.component';
import { SelectionItemPopupComponent } from './selection-item-popup/selection-item.popup.component';

@Component({
  templateUrl: './custom-attribute-list.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }
  `],
  animations: egretAnimations
})
export class CustomAttributeListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected dialog: MatDialog,
    protected customAttributeService: CustomAttributeService,
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
      extendableTypeName: [''],
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

    self.customAttributeService.getCustomAttributes(self.viewModel.extendableTypeName, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching attributes");
        });
  }

  openAddPopUp() {
    let self = this;
    let title = 'Add Attribute';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CustomAttributeAddPopupComponent, {
      width: '920px',
      minHeight: '530px',
      disableClose: true,
      data: { customAttributeId: 0, extendableTypeName: self.viewModel.extendableTypeName, title: title }
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

  openEditPopUp(data: any = {}) {
    let self = this;
    let title = 'Update Attribute';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CustomAttributeEditPopupComponent, {
      width: '920px',
      minHeight: '530px',
      disableClose: true,
      data: { customAttributeId: data.id, extendableTypeName: self.viewModel.extendableTypeName, title: title }
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
    let title = 'Delete Attribute';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CustomAttributeDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { customAttributeId: data.id, attributeKey: data.attributeKey, title: title }
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

  openSelectionPopUp(data: any = {}) {
    let self = this;
    let title = 'Update Selection Values';
    let dialogRef: MatDialogRef<any> = self.dialog.open(SelectionItemPopupComponent, {
      width: '920px',
      minHeight: '530px',
      disableClose: true,
      data: { customAttributeId: data.id, title: title }
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

  selectExtendableType(extendableTypeName: string): void {
    this.viewModel.extendableTypeName = extendableTypeName;
    this.loadGrid();
  }  
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['id', 'attribute-key', 'category', 'attribute-type', 'actions']);
  
  extendableTypeName: string = '1';
}

class GridRecordModel {
  id: number;
  extendableTypeName: string;
  attributeKey: string;
  category: string;
  customAttributeType: string;
}