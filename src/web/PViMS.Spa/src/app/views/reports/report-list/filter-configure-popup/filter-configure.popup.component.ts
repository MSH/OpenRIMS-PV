import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { GridModel } from 'app/shared/models/grid.model';
import { MetaService } from 'app/shared/services/meta.service';
import { MetaReportService } from 'app/shared/services/meta-report.service';
import { MetaAttributeModel } from 'app/shared/models/meta/meta-attribute.model';
import { MetaFilterModel } from 'app/shared/models/meta/meta-filter.model';
import { MetaTableExpandedModel } from 'app/shared/models/meta/meta-table.expanded.model';
import { MetaColumnExpandedModel } from 'app/shared/models/meta/meta-column.expanded.model';

@Component({
  templateUrl: './filter-configure.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FilterConfigurePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FilterConfigurePopupData,
    public dialogRef: MatDialogRef<FilterConfigurePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected metaReportService: MetaReportService,
    protected metaService: MetaService,
    protected popupService: PopupService,
    protected accountService: AccountService) 
  { 
    super(_router, _location, popupService, accountService);
  }

  viewModel: ViewModel = new ViewModel();
  attributes: MetaAttributeModel[] = [];
  filters: MetaFilterModel[] = [];
  coreMetaTable: MetaTableExpandedModel;
  selectedColumn: MetaColumnExpandedModel;

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      reportName: [self.data.reportName || ''],
      reportType: [''],
      coreEntity: [''],
      relation: ['And'],
      attributeName: [''],
      operator: ['']
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.metaReportId > 0) {
      self.loadData();
    }
  }   

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaReportService.getMetaReportByExpanded(self.data.metaReportId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, result);

        self.attributes = result.attributes;
        self.filters = result.filters;
        self.coreMetaTable = result.coreMetaTable;

        self.viewModel.filterGrid.updateBasic(result.filters);
      }, error => {
        self.handleError(error, "Error fetching meta report");
      });
  }   

  submit() {
    let self = this;
    self.setBusy(true);

    self.metaReportService.saveMetaReportAttributes(self.data.metaReportId, self.itemForm.value, self.attributes, self.filters)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Meta report filters saved successfully", "Meta Reports");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error saving meta report");
    });
  }

  changeColumn(e) {
    if (e.source.value != '') {
      let self = this;
      self.selectedColumn = self.coreMetaTable.columns.find(mc => mc.columnName == e.source.value)
     }
  }

  addFilter(): void {
    let self = this;
    
    if(self.itemForm.get('relation').value == '' 
    || self.itemForm.get('attributeName').value == ''
    || self.itemForm.get('operator').value == '') {
      return;
    }

    let selectedRowIndex = self.filters.findIndex(a => a.attributeName == self.itemForm.get('attributeName').value);
    if(selectedRowIndex > - 1) {
      return;
    }

    let index = self.filters.length + 1;
    let filter: FilterGridRecordModel = {
      index: index,
      relation: self.itemForm.get('relation').value,
      attributeName: self.itemForm.get('attributeName').value,
      operator: self.itemForm.get('operator').value,
      columnType: ''
    }
    self.filters.push(filter);
    self.viewModel.filterGrid.updateBasic(self.filters);
  }  

  removeFilter(index: number): void {
    let self = this;

    let selectedRowIndex = self.filters.findIndex(a => a.index == index);
    self.filters.splice(selectedRowIndex, 1);
    self.viewModel.filterGrid.updateBasic(self.filters);
  }  
}

export interface FilterConfigurePopupData {
  metaReportId: number;
  title: string;
  reportName: string;
}

class ViewModel {
  filterGrid: GridModel<FilterGridRecordModel> =
    new GridModel<FilterGridRecordModel>
        (['relation', 'attribute-name', 'operator', 'actions']);
}

class FilterGridRecordModel {
  index: number;
  relation: string;
  attributeName: string;
  columnType: string;
  operator: string;
}
