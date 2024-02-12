import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { finalize } from 'rxjs/operators';
import { GridModel } from 'app/shared/models/grid.model';
import { SelectionDataItemModel } from 'app/shared/models/custom-attribute/selection-data-item.model';

@Component({
  templateUrl: './selection-item.popup.component.html',
  styles: [`
    .mat-column-selection-key { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-actions { flex: 0 0 20% !important; width: 20% !important; }
  `],
  animations: egretAnimations
})
export class SelectionItemPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  viewModel: ViewModel = new ViewModel();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<SelectionItemPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected customAttributeService: CustomAttributeService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      attributeKey: [''],
      selectionKey: ['', [Validators.required, Validators.maxLength(5), Validators.pattern('[0-9]*')]],
      dataItemValue: ['', [Validators.required, Validators.maxLength(150)]],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.selectionGrid.setupBasic(null, null, null);
    if (self.data.customAttributeId > 0) {
        self.loadData();
    }
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.customAttributeService.getCustomAttributeDetail(self.data.customAttributeId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, { attributeKey: result.attributeKey });
        self.viewModel.selectionGrid.updateBasic(result.selectionDataItems);
      }, error => {
        self.handleError(error, "Error fetching attribute");
      });
  }

  deleteValue(data: SelectionDataItemModel) {
    let self = this;
    self.setBusy(true);

    self.customAttributeService.deleteSelectionValue(self.data.customAttributeId, data.selectionKey)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Selection value deleted successfully", "Attributes");
        self.loadData();
      }, error => {
        self.handleError(error, "Error deleting value");
      });
  }

  saveValue() {
    let self = this;
    self.setBusy(true);

    self.customAttributeService.saveSelectionValue(self.data.customAttributeId, self.itemForm.value)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Selection value saved successfully", "Attributes");
      self.loadData();
    }, error => {
      self.handleError(error, "Error saving selection value");
   });
  }
}

export interface PopupData {
  customAttributeId: number;
  title: string;
}

class ViewModel {
  selectionGrid: GridModel<SelectionDataItemModel> =
  new GridModel<SelectionDataItemModel>
      (['selection-key', 'value', 'actions']);

  customAttribute: CustomAttributeDetailModel;
}