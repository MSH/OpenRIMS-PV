import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { FormPopupComponent } from './form-popup/form.popup.component';
import { _routes } from 'app/config/routes';

@Component({
  templateUrl: './form-list.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-form-name { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-system { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }  
  `],  
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
  }

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

    self.metaFormService.getMetaForms(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  openFormPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Form' : 'Update Form';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId: isNew ? 0: data.id, title: title, payload: data }
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

  navigateToForm(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.administration.work.form(model.id)]);
  }  

}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['id', 'form-name', 'unique-identifier', 'cohort-group', 'system', 'action-name', 'actions']);
}

class GridRecordModel {
  id: number;
  system: string;
  cohortGroup: string;
  formName: string;
  actionName: string;
  metaFormGuid: string;
}