import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { finalize } from 'rxjs/operators';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { DatasetCategoryViewModel } from 'app/shared/models/dataset/dataset-category-view.model';
import { DatasetService } from 'app/shared/services/dataset.service';

@Component({
  templateUrl: './dataset-instance.popup.component.html',
  animations: egretAnimations
})
export class DatasetInstancePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: DatasetInstancePopupData,
    public dialogRef: MatDialogRef<DatasetInstancePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected datasetService: DatasetService,
    protected customAttributeService: CustomAttributeService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  datasetCategories: DatasetCategoryViewModel[] = [];
  
  ngOnInit(): void {
    const self = this;

    self.viewModelForm = this._formBuilder.group({
      elements: this._formBuilder.group([]),
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.instanceId > 0) {
      self.loadData();
    }
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.datasetService.getDatasetInstanceDetail(self.data.datasetId, self.data.instanceId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        // handle dynamic data
        self.datasetCategories = result.datasetCategories;
        // Add elements to form group
        let elements = self.viewModelForm.get('elements') as FormGroup;
        self.datasetCategories.forEach(category => {
          category.datasetElements.forEach(element => {
            let validators = [ ];
            if(element.required) {
               validators.push(Validators.required);
            }
            if(element.stringMaxLength != null) {
               validators.push(Validators.maxLength(element.stringMaxLength));
            }
            if(element.numericMinValue != null && element.numericMaxValue != null) {
               validators.push(Validators.max(element.numericMaxValue));
               validators.push(Validators.min(element.numericMinValue));
            }

            elements.addControl(element.datasetElementId.toString(), new FormControl(element.datasetElementValue, validators));
          })
        })
        
      }, error => {
        self.throwError(error, error.statusText);
      });
  }   

  submit() {
    let self = this;
    self.setBusy(true);

    // console.log(self.viewModelForm.value);
    // self.encounterService.saveEncounter(self.data.patientId, self.data.encounterId, self.viewModelForm.value)
    //   .pipe(finalize(() => self.setBusy(false)))
    //   .subscribe(result => {
    //     self.notify("Encounter successfully updated!", "Success");
    //     this.dialogRef.close(this.viewModelForm.value);
    // }, error => {
    //   if(error.status == 400) {
    //     self.showInfo(error.message[0], error.statusText);
    //   } else {
    //     if(Array.isArray(error.message)) {
    //       self.showInfo(error.message[0], error.statusText);
    //     }
    //     else {
    //       self.throwError(error, error.statusText);
    //     }
    //   }
    // });      
  }
}

export interface DatasetInstancePopupData {
  datasetId: number;
  instanceId: number;
  title: string;
}