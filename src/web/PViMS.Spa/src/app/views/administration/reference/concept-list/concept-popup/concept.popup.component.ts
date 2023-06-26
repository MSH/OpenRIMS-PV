import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ConceptService } from 'app/shared/services/concept.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { MedicationFormIdentifierModel } from 'app/shared/models/concepts/medication-form.identifier.model';

@Component({
  selector: 'concept-popup',
  templateUrl: './concept.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class ConceptPopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  formList: MedicationFormIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ConceptPopupData,
    public dialogRef: MatDialogRef<ConceptPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected conceptService: ConceptService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this._formBuilder.group({
      conceptName: [this.data.payload.conceptName || '', [Validators.required, Validators.maxLength(1000), Validators.pattern('[-a-zA-Z0-9 .,()%/]*')]],
      strength: [this.data.payload.strength || '', [Validators.maxLength(250), Validators.pattern('[-a-zA-Z0-9 ,()/]*')]],
      medicationForm: [this.data.payload.formName || '', Validators.required],
      active: [this.data.payload.active, Validators.required]
    })
  }

  loadDropDowns(): void {
    let self = this;
    self.getMedicationFormList();
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.conceptService.saveConcept(self.data.conceptId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Active ingredient saved successfully", "Medications");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        self.handleError(error, "Error saving active ingredient");
    });
  }

  private getMedicationFormList(): void {
    let self = this;
    self.conceptService.getAllMedicationForms()
      .subscribe(result => {
        self.formList = result;
      }, error => {
        self.handleError(error, "Error fetching medication forms");
      });
  }
}

export interface ConceptPopupData {
  conceptId: number;
  title: string;
  payload: any;
}