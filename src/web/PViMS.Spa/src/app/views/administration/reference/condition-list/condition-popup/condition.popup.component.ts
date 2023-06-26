import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { ConceptSelectPopupComponent } from 'app/shared/components/popup/concept-select-popup/concept-select.popup.component';
import { ConditionService } from 'app/shared/services/condition.service';
import { GridModel } from 'app/shared/models/grid.model';
import { LabTestSelectPopupComponent } from 'app/views/administration/shared/lab-test-select-popup/lab-test-select.popup.component';
import { MeddraSelectPopupComponent } from 'app/shared/components/popup/meddra-select-popup/meddra-select.popup.component';

@Component({
  templateUrl: './condition.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class ConditionPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ConditionPopupData,
    public dialogRef: MatDialogRef<ConditionPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected conditionService: ConditionService,
    protected accountService: AccountService,
    protected popupService: PopupService,
    protected dialog: MatDialog
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  viewGridModel: ViewGridModel;

  labTests: LabTestGridRecordModel[] = [];
  meddras: MeddraGridRecordModel[] = [];
  medications: MedicationGridRecordModel[] = [];

  ngOnInit(): void {
    const self = this;

    self.viewGridModel = new ViewGridModel();

    self.itemForm = this._formBuilder.group({
      conditionName: [this.data.payload.conditionName || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]],
      active: ['', Validators.required],
      chronic: ['', Validators.required],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.conditionId > 0) {
      self.viewGridModel.labTestGrid.setupBasic(null, null, null);
      self.viewGridModel.meddraGrid.setupBasic(null, null, null);
      self.viewGridModel.medicationGrid.setupBasic(null, null, null);

      self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.conditionService.getConditionDetail(self.data.conditionId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        console.log(result);
        self.updateForm(self.itemForm, (self.data.payload = result));

        self.labTests = result.conditionLabTests;
        self.viewGridModel.labTestGrid.updateBasic(self.labTests);

        self.meddras = result.conditionMedDras;
        self.viewGridModel.meddraGrid.updateBasic(self.meddras);

        self.medications = result.conditionMedications;
        self.viewGridModel.medicationGrid.updateBasic(self.medications);
      }, error => {
        self.handleError(error, "Error fetching condition")
      });
  }  

  openConceptSelectPopup() {
    let self = this;
    let title = 'Select Medication';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ConceptSelectPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title, productOnly: true }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        let medication: MedicationGridRecordModel = {
          id: 0,
          productId: result.id,
          medicationName: result.displayName
        };
        self.medications.push(medication);
        self.viewGridModel.medicationGrid.updateBasic(self.medications);
      })
  }  

  removeMedication(productId: number): void {
    let self = this;
    self.medications.splice(self.medications.findIndex(m => m.productId == productId), 1)
    this.viewGridModel.medicationGrid.updateBasic(this.medications);

    this.notify("Medication removed successfully!", "Medication");
  }

  openLabTestSelectPopup() {
    let self = this;
    let title = 'Select Test or Procedure';
    let dialogRef: MatDialogRef<any> = self.dialog.open(LabTestSelectPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        let labTest: LabTestGridRecordModel = {
          id: 0,
          labTestId: result.id,
          labTestName: result.labTestName
        };
        self.labTests.push(labTest);
        self.viewGridModel.labTestGrid.updateBasic(self.labTests);
      })
  }

  removeLabTest(labTestId: number): void {
    let self = this;
    self.labTests.splice(self.labTests.findIndex(l => l.labTestId == labTestId), 1)
    this.viewGridModel.labTestGrid.updateBasic(this.labTests);

    this.notify("Lab Test removed successfully!", "Lab Test");
  }

  openMeddraSelectPopup() {
    let self = this;
    let title = 'Select MedDra Term';
    let dialogRef: MatDialogRef<any> = self.dialog.open(MeddraSelectPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        let meddra: MeddraGridRecordModel = {
          id: 0,
          terminologyMedDraId: result.id,
          medDraTerm: result.medDraTerm
        };
        self.meddras.push(meddra);
        self.viewGridModel.meddraGrid.updateBasic(self.meddras);
      })
  }

  removeMeddra(terminologyMedDraId: number): void {
    let self = this;
    self.meddras.splice(self.meddras.findIndex(m => m.terminologyMedDraId == terminologyMedDraId), 1)
    this.viewGridModel.meddraGrid.updateBasic(this.meddras);

    this.notify("MedDra Term removed successfully!", "MedDra Term");
  }

  submit() {
    let self = this;
    self.setBusy(true);

    const conditionLabTestIds = [];
    self.labTests.forEach(labTest => conditionLabTestIds.push(labTest.labTestId));
    const conditionMeddraIds = [];
    self.meddras.forEach(meddra => conditionMeddraIds.push(meddra.terminologyMedDraId));
    const conditionMedicationIds = [];
    self.medications.forEach(medication => conditionMedicationIds.push(medication.productId));

    self.conditionService.saveCondition(self.data.conditionId, self.itemForm.value, conditionLabTestIds, conditionMeddraIds, conditionMedicationIds)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Condition saved successfully", "Conditions");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error saving condition");
    });
  }
}

export interface ConditionPopupData {
  conditionId: number;
  title: string;
  payload: any;
}

class ViewGridModel {
  labTestGrid: GridModel<LabTestGridRecordModel> =
  new GridModel<LabTestGridRecordModel>
      (['test', 'actions']);

  meddraGrid: GridModel<MeddraGridRecordModel> =
  new GridModel<MeddraGridRecordModel>
      (['term', 'actions']);
    
  medicationGrid: GridModel<MedicationGridRecordModel> =
  new GridModel<MedicationGridRecordModel>
      (['medication', 'actions']);
}

class LabTestGridRecordModel {
  id: number;
  labTestId: number;
  labTestName: string;
}

class MeddraGridRecordModel {
  id: number;
  terminologyMedDraId: number;
  medDraTerm: string;
}

class MedicationGridRecordModel {
  id: number;
  productId: number;
  medicationName: string;
}
