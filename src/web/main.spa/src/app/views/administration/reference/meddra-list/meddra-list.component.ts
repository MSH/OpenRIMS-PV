import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MeddraTermService } from 'app/shared/services/meddra-term.service';
import { ImportMeddraPopupComponent } from './import-meddra-popup/import-meddra.popup.component';

@Component({
  templateUrl: './meddra-list.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-code { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-term-type { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-version { flex: 0 0 10% !important; width: 10% !important; }
  `],  
  animations: egretAnimations
})
export class MeddraListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected meddraTermService: MeddraTermService,
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
      termType: ['SOC'],
      searchTerm: ['', [Validators.minLength(3), Validators.maxLength(100), Validators.pattern("[-a-zA-Z0-9 ']*")]],
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

    self.meddraTermService.searchTermsByDetail(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  openImportPopUp() {
    let self = this;
    let title = 'Import MedDra';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ImportMeddraPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
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
          (['id', 'parent-term', 'term', 'code', 'term-type', 'version']);
  
  termType: string;
  searchTerm: string;
}

class GridRecordModel {
  id: number;
  medDraTerm: string;
  medDraCode: string;
  parentMedDraTerm: string;
  medDraTermType: string;
  medDraVersion: string;
}
