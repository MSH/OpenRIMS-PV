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
import { EncounterService } from 'app/shared/services/encounter.service';
import { DatasetCategoryViewModel } from 'app/shared/models/dataset/dataset-category-view.model';

@Component({
  templateUrl: './encounter-update.popup.component.html',
  animations: egretAnimations
})
export class EncounterUpdatePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public viewModelForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EncounterUpdatePopupData,
    public dialogRef: MatDialogRef<EncounterUpdatePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected encounterService: EncounterService,
    protected customAttributeService: CustomAttributeService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  datasetCategories: DatasetCategoryViewModel[] = [];
  
  ngOnInit(): void {
    const self = this;

    self.viewModelForm = this._formBuilder.group({
      elements: this._formBuilder.group([]),
      notes: [''],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.encounterId > 0) {
      self.loadData();
    }
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.encounterService.getEncounterExpanded(self.data.patientId, self.data.encounterId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);

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
        self.handleError(error, "Error fetching encounter");
      });
  }   

  submit() {
    let self = this;
    self.setBusy(true);

    self.encounterService.saveEncounter(self.data.patientId, self.data.encounterId, self.viewModelForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Encounter successfully updated!", "Success");
        this.dialogRef.close(this.viewModelForm.value);
    }, error => {
      self.handleError(error, "Error saving encounter");
    });      
  }
}

export interface EncounterUpdatePopupData {
  patientId: number;
  encounterId: number;
  title: string;
}