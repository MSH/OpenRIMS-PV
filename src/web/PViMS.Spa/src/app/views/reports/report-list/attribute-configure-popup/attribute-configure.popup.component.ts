import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
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

@Component({
  templateUrl: './attribute-configure.popup.component.html',
  animations: egretAnimations
})
export class AttributeConfigurePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AttributeConfigurePopupData,
    public dialogRef: MatDialogRef<AttributeConfigurePopupComponent>,
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

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      reportName: [self.data.reportName || ''],
      reportType: [''],
      coreEntity: [''],
      attributeName: [''],
      aggregate: [''],
      displayName: ['', [Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]]
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

        self.viewModel.attributeGrid.updateBasic(result.attributes);
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
        self.notify("Meta report attributes saved successfully", "Meta Reports");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error saving meta report");
    });
  }

  addAttribute(): void {
    let self = this;
    
    if(self.itemForm.get('attributeName').value == '') {
      return;
    }
    if(self.itemForm.get('reportType').value == 'Summary' 
        && self.itemForm.get('aggregate').value == '') {
      return;
    }
    let displayName = self.itemForm.get('displayName').value;
    if(self.itemForm.get('displayName').value.trim() == '') {
      displayName = self.itemForm.get('attributeName').value;
    }

    let selectedRowIndex = self.attributes.findIndex(a => a.attributeName == self.itemForm.get('attributeName').value);
    if(selectedRowIndex > - 1) {
      return;
    }

    let index = self.attributes.length + 1;
    let attribute: AttributeGridRecordModel = {
      index: index,
      attributeName: self.itemForm.get('attributeName').value,
      aggregate: self.itemForm.get('aggregate').value,
      displayName: displayName
    }
    self.attributes.push(attribute);
    self.viewModel.attributeGrid.updateBasic(self.attributes);
  }  

  removeAttribute(index: number): void {
    let self = this;

    let selectedRowIndex = self.attributes.findIndex(a => a.index == index);
    self.attributes.splice(selectedRowIndex, 1);
    self.viewModel.attributeGrid.updateBasic(self.attributes);
  }  
}

export interface AttributeConfigurePopupData {
  metaReportId: number;
  title: string;
  reportName: string;
}

class ViewModel {
  attributeGrid: GridModel<AttributeGridRecordModel> =
    new GridModel<AttributeGridRecordModel>
        (['attribute-name', 'aggregate', 'display-name', 'actions']);
}

class AttributeGridRecordModel {
  index: number;
  attributeName: string;
  aggregate: string;
  displayName: string;
}
