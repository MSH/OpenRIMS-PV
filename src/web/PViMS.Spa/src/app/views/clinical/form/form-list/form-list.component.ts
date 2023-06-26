import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from 'app/shared/services/event.service';
import { AccountService } from 'app/shared/services/account.service';
import { PopupService } from 'app/shared/services/popup.service';
import { BaseComponent } from 'app/shared/base/base.component';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { _routes } from 'app/config/routes';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { AttachmentCapturePopupComponent } from './attachment-capture-popup/attachment-capture.popup.component';
import { AttachmentViewPopupComponent } from './attachment-view-popup/attachment-view.popup.component';
import { FormDeletePopupComponent } from './form-delete-popup/form-delete.popup.component';

@Component({
  templateUrl: './form-list.component.html',
  animations: egretAnimations
})
export class FormListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected metaFormService: MetaFormService,
    protected dialog: MatDialog,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          // this.setupTable();
      }
    });    
  }

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  synchRequired = false;

  ngOnInit(): void {
    const self = this;

    self.viewModel.formType = self._activatedRoute.snapshot.paramMap.get('type');

    self.viewModelForm = self._formBuilder.group({
      searchTerm: [this.viewModel.searchTerm],
      synchForms: [this.viewModel.synchForms],
      compForms: [this.viewModel.compForms],
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.incompleteGrid.setupBasic(null, null, null);
    self.viewModel.completeGrid.setupBasic(null, null, null);
    self.viewModel.synchronisedGrid.setupBasic(null, null, null);
    self.viewModel.searchGrid.setupBasic(null, null, null);
    self.loadGrids();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormListComponent.name);
  } 

  loadGrids(): void {
    let self = this;
    self.loadIncompleteGrid();
    self.loadCompleteGrid();
    self.loadSynchronisedGrid();
  }

  loadIncompleteGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getFilteredFormsByType(self.viewModel.formType, false, false).then(result => {
      self.viewModel.incompleteGrid.updateBasic(result.value);
      self.setBusy(false);
    }, error => {
      self.throwError(error, error.statusText);
    });
  }

  loadCompleteGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getFilteredFormsByType(self.viewModel.formType, false, true).then(result => {
      self.viewModel.completeGrid.updateBasic(result.value);
      self.setBusy(false);
    }, error => {
      self.throwError(error, error.statusText);
    });
  }  

  loadSynchronisedGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getFilteredFormsByType(self.viewModel.formType, true, true).then(result => {
      self.viewModel.synchronisedGrid.updateBasic(result.value);
      self.setBusy(false);
    }, error => {
      self.throwError(error, error.statusText);
    });
  }  

  loadSearchGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.searchFormsByType(self.viewModel.formType, self.viewModelForm.get('searchTerm').value).then(result => {
      self.viewModel.searchGrid.updateBasic(result.value);
      self.setBusy(false);
    }, error => {
      self.throwError(error, error.statusText);
    });
  }    

  viewForm(model: GridRecordModel = null): void {
    const self = this;
    console.log(model);
    switch(model.formType) { 
      case 'FormA': { 
        self._router.navigate([_routes.clinical.forms.viewFormA(model.id)]);
         break; 
      } 
      case 'FormB': { 
        self._router.navigate([_routes.clinical.forms.viewFormB(model.id)]);
         break; 
      } 
      case 'FormC': { 
        self._router.navigate([_routes.clinical.forms.viewFormC(model.id)]);
         break; 
      } 
      case 'FormADR': { 
        self._router.navigate([_routes.clinical.forms.viewFormADR(model.id)]);
         break; 
      } 
   }     
  }

  openCameraPopup(id: number, index: number) {
    let self = this;
    let title = "Capture Image";
    let dialogRef: MatDialogRef<any> = self.dialog.open(AttachmentCapturePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId: id, title, index }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrids();
      })
  }   

  openImageView(id: number, index: number) {
    let self = this;
    let title = "View Image";
    let dialogRef: MatDialogRef<any> = self.dialog.open(AttachmentViewPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId: id, title, index }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrids();
      })
  }

  openFormDelete(id: number) {
    let self = this;
    let title = "Delete Form";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { id: id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrids();
      })
  }   
}

class ViewModel {
  incompleteGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['created', 'form-type', 'patient identifier', 'patient name', 'complete status', 'synch status' , 'actions']);

  completeGrid: GridModel<GridRecordModel> =
  new GridModel<GridRecordModel>
      (['created', 'form-type', 'identifier', 'patient identifier', 'patient name', 'complete status', 'synch status' , 'actions']);

  synchronisedGrid: GridModel<GridRecordModel> =
  new GridModel<GridRecordModel>
      (['created', 'form-type', 'identifier', 'patient identifier', 'patient name', 'complete status', 'synch status' , 'actions']);

  searchGrid: GridModel<GridRecordModel> =
  new GridModel<GridRecordModel>
      (['created', 'form-type', 'identifier', 'patient identifier', 'patient name', 'complete status', 'synch status' , 'actions']);
          
  formType: string;
  searchTerm: string;
  synchForms: any;
  compForms: any;
}

class GridRecordModel {
  id: number;
  created: string;
  formIdentifier: string;
  patientIdentifier: string;
  patientName: string;
  completeStatus: string;
  synchStatus: string;
  formType: string;
  hasAttachment: boolean;
  hasSecondAttachment: boolean;
}