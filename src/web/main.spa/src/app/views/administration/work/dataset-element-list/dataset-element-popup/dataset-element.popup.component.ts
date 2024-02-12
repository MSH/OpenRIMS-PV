import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { DatasetElementService } from 'app/shared/services/dataset-element.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './dataset-element.popup.component.html',
  animations: egretAnimations
})
export class DatasetElementPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<DatasetElementPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected datasetElementService: DatasetElementService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      elementName: [this.data.payload.elementName || '', [Validators.required, Validators.maxLength(100), Validators.pattern('[a-zA-Z() ]*')]],
      oid: ['', [Validators.maxLength(50), Validators.pattern('[-a-zA-Z0-9 ]*')]],
      defaultValue: ['', [Validators.maxLength(150), Validators.pattern('[-a-zA-Z0-9 ]*')]],
      mandatory: ['', Validators.required],
      anonymise: ['', Validators.required],
      system: ['', Validators.required],
      singleDatasetRule: ['', Validators.required],
      fieldTypeName: [this.data.payload.fieldTypeName || '', Validators.required],
      maxLength: [''],
      decimals: [''],
      minSize: [''],
      maxSize: [''],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.datasetElementId > 0) {
        self.loadData();
    }
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.datasetElementService.getDatasetElementExpanded(self.data.datasetElementId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        self.handleError(error, "Error fetching dataset element");
      });
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.datasetElementService.saveDatasetElement(self.data.datasetElementId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Dataset element saved successfully", "Dataset Elements");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error saving dataset element");      
    });
  }
}

export interface PopupData {
  datasetElementId: number;
  title: string;
  payload: any;
}