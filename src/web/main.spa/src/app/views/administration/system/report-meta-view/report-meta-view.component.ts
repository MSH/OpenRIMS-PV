import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Subscription, forkJoin } from 'rxjs';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { GridModel } from 'app/shared/models/grid.model';
import { MetaService } from 'app/shared/services/meta.service';
import { takeUntil, finalize, tap } from 'rxjs/operators';
import { MetaSummaryModel } from 'app/shared/models/meta/meta-summary.model';

@Component({
  templateUrl: './report-meta-view.component.html',
  animations: egretAnimations  
})
export class ReportMetaViewComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected mediaObserver: MediaObserver,
    protected metaService: MetaService,
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

  viewModel: ViewModel = new ViewModel();;
  viewModelForm: FormGroup;

  @ViewChild('tableGridPaginator') tableGridPaginator: MatPaginator;
  @ViewChild('columnGridPaginator') columnGridPaginator: MatPaginator;
  @ViewChild('dependencyGridPaginator') dependencyGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
      tableCount: [this.viewModel.tableCount],
      columnCount: [this.viewModel.columnCount],
      dependencyCount: [this.viewModel.dependencyCount],
      patientCount: [this.viewModel.patientCount],
      latestRefreshDate: [this.viewModel.latestRefreshDate]
    });
  }

  ngAfterViewInit(): void {
    let self = this;

    self.viewModel.tableGrid.setupAdvance(
      null, null, self.tableGridPaginator)
      .subscribe(() => { });

    self.viewModel.columnGrid.setupAdvance(
      null, null, self.columnGridPaginator)
      .subscribe(() => { });

    self.viewModel.dependencyGrid.setupAdvance(
      null, null, self.dependencyGridPaginator)
      .subscribe(() => { });
    
    self.loadData();
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    forkJoin(
      self.metaService.getMetaTables(self.viewModel.tableGrid.customFilterModel(self.viewModelForm.value)),
      self.metaService.getMetaColumns(self.viewModel.columnGrid.customFilterModel(self.viewModelForm.value)),
      self.metaService.getMetaDependencies(self.viewModel.dependencyGrid.customFilterModel(self.viewModelForm.value)),
      self.metaService.getMetaSummary()
    )
    .pipe(takeUntil(self._unsubscribeAll))
    .pipe(finalize(() => self.setBusy(false)))
    .pipe(tap(result => {
      let summary = result[3] as MetaSummaryModel;
      self.viewModel.patientCount = summary.patientCount;
      self.viewModel.latestRefreshDate = summary.latestRefreshDate;
    }))        
    .subscribe(result => {
      self.viewModel.tableGrid.updateAdvance(result[0]);
      self.viewModel.tableCount = self.viewModel.tableGrid.count;

      self.viewModel.columnGrid.updateAdvance(result[1]);
      self.viewModel.columnCount = self.viewModel.columnGrid.count;

      self.viewModel.dependencyGrid.updateAdvance(result[2]);
      self.viewModel.dependencyCount = self.viewModel.dependencyGrid.count;

      self.updateForm(self.viewModelForm, self.viewModel);
    }, error => {
        self.handleError(error, error.statusText);
    });
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(ReportMetaViewComponent.name);
  }
}

class ViewModel {
  tableCount: number;
  columnCount: number;
  dependencyCount: number;
  patientCount: number;
  latestRefreshDate: string;

  tableGrid: GridModel<TableGridRecordModel> =
    new GridModel<TableGridRecordModel>
        (['table-name', 'friendly-name', 'description', 'type']);
  
  columnGrid: GridModel<ColumnGridRecordModel> =
    new GridModel<ColumnGridRecordModel>
        (['table-name', 'column-name', 'identity', 'nullable', 'type']);
    
  dependencyGrid: GridModel<DependencyGridRecordModel> =
    new GridModel<DependencyGridRecordModel>
        (['parent-table-name', 'parent-column-name', 'reference-table-name', 'reference-column-name']);
}

class TableGridRecordModel {
  id: number;
  tableName: string;
  friendlyName: string;
  friendlyDescription: string;
  tableType: string;
}

class ColumnGridRecordModel {
  id: number;
  tableName: string;
  columnName: string;
  isIdentity: string;
  isNullable: string;
  columnType: string;
}

class DependencyGridRecordModel {
  id: number;
  parentTableName: string;
  parentColumnName: string;
  referenceTableName: string;
  referenceColumnName: string;
}