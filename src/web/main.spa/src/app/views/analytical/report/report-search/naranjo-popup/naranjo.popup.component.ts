import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTabGroup } from '@angular/material/tabs';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';
import { CustomAttributeDetailModel } from 'app/shared/models/custom-attribute/custom-attribute.detail.model';
import { PatientService } from 'app/shared/services/patient.service';
import { finalize } from 'rxjs/operators';
import { AttributeValueModel } from 'app/shared/models/attributevalue.model';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { GridModel } from 'app/shared/models/grid.model';
import { ReportInstanceMedicationDetailModel } from 'app/shared/models/report-instance/report-instance-medication.detail.model';

@Component({
  templateUrl: './naranjo.popup.component.html',
  styles: [`
    .mat-column-legend-term { flex: 0 0 30% !important; width: 30% !important; }
    .mat-column-legend-description { flex: 0 0 60% !important; width: 60% !important; }
    .mat-column-medication { flex: 0 0 35% !important; width: 35% !important; }
    .mat-column-start-date { flex: 0 0 12% !important; width: 12% !important; }
    .mat-column-end-date { flex: 0 0 12% !important; width: 12% !important; }
    .mat-column-causality { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }
  `],  
  animations: egretAnimations
})
export class NaranjoPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();
  public viewModelForm: FormGroup;

  customAttributeList: CustomAttributeDetailModel[] = [];
  clinicalEventAttributes: AttributeValueModel[];
  
  arrayAttributes: {
    id: number;
    value: string;
  }[];

  @ViewChild('mainTabGroup') mainTabGroup: MatTabGroup;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: NaranjoPopupData,
    public dialogRef: MatDialogRef<NaranjoPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService,
    protected customAttributeService: CustomAttributeService,
    protected reportInstanceService: ReportInstanceService
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  calculation: string = '';
  score: number;
  legendDataSource = LEGEND_DATA;
  selectedMedication: ReportInstanceMedicationDetailModel = null;

  ngOnInit(): void {
    const self = this;

    self.arrayAttributes = [];
    self.viewModelForm = this._formBuilder.group({
      sourceIdentifier: [''],
      meddraTerm: [''],
      question1: [''],
      question2: [''],
      question3: [''],
      question4: [''],
      question5: [''],
      question6: [''],
      question7: [''],
      question8: [''],
      question9: [''],
      question10: [''],
      causalityConfigType: ['2'],
      causality: [''],
      score: ['']
    })

    this.calculation = '';
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.reportInstanceId > 0) {
      self.viewModel.medicationGrid.setupBasic(null, null, null);

      self.loadData();
    }
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.reportInstanceService.getReportInstanceDetail(self.data.workFlowId, self.data.reportInstanceId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.viewModelForm, result);
        self.updateForm(self.viewModelForm, {meddraTerm: result.terminologyMedDra.medDraTerm});

        self.viewModel.medicationGrid.updateBasic(result.medications);
      }, error => {
        this.handleError(error, "Error loading report instance");
      });
  }   

  calculate() {
    let self = this;

    if (self.viewModelForm.get('question1').value == '' 
        || self.viewModelForm.get('question2').value == ''  
        || self.viewModelForm.get('question3').value == '' 
        || self.viewModelForm.get('question4').value == ''  
        || self.viewModelForm.get('question5').value == ''
        || self.viewModelForm.get('question6').value == ''
        || self.viewModelForm.get('question7').value == ''
        || self.viewModelForm.get('question8').value == ''
        || self.viewModelForm.get('question9').value == ''
        || self.viewModelForm.get('question10').value == '') {
          self.calculation = '';
      return;
    }

    var q1 = self.viewModelForm.get('question1').value == "Yes" ? 1 : 0;
    var q3 = self.viewModelForm.get('question3').value == "Yes" ? 1 : 0;
    var q7 = self.viewModelForm.get('question7').value == "Yes" ? 1 : 0;
    var q8 = self.viewModelForm.get('question8').value == "Yes" ? 1 : 0;
    var q9 = self.viewModelForm.get('question9').value == "Yes" ? 1 : 0;
    var q10 = self.viewModelForm.get('question10').value == "Yes" ? 1 : 0;

    var q2 = 0;
    switch (self.viewModelForm.get('question2').value) {
      case "Yes":
        q2 = 2;
        break;
      case "No":
        q2 = -1;
    }
    var q4 = 0;
    switch (self.viewModelForm.get('question4').value) {
      case "Yes":
        q4 = 2;
        break;
      case "No":
        q4 = -1;
    }
    var q5 = 0;
    switch (self.viewModelForm.get('question5').value) {
      case "Yes":
        q5 = -1;
        break;
      case "No":
        q5 = 2;
    }
    var q6 = 0;
    switch (self.viewModelForm.get('question6').value) {
      case "Yes":
        q6 = -1;
        break;
      case "No":
        q6 = 1;
    }
    var q7 = 0;
    switch (self.viewModelForm.get('question7').value) {
      case "Yes":
        q7 = -1;
        break;
      case "No":
        q6 = 1;
    }

    var score = q1 + q2 + q3 + q4 + q5 + q6 + q7 + q8 + q9 + q10;
    self.score = score;
    console.log(score);

    // Calculate causality
    switch (true) {
      case (score >= 9):
        self.calculation = 'Definite';
          break;
      case (score > 4 && score < 9):
        self.calculation = 'Probable';
          break;
      case (score > 0 && score < 5):
        self.calculation = 'Possible';
          break;
      case (score <= 0):
        self.calculation = 'Doubtful';
          break;
    }
  }

  openCausality(data: any = {}) {
    this.selectedMedication = data;
    this.mainTabGroup.selectedIndex = 4;
  }

  setCausality() {
    let self = this;
    self.setBusy(true);

    self.updateForm(self.viewModelForm, {causality: self.calculation});
    self.updateForm(self.viewModelForm, {score: self.score});

    self.reportInstanceService.updateReportInstanceMedicationCausality(self.data.workFlowId, self.data.reportInstanceId, self.selectedMedication.id, self.viewModelForm.value)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Causality set successfully", "Activity");
      self.initForm();
      self.loadData();
      self.selectedMedication = null;
    }, error => {
      this.handleError(error, "Error updating causality");
    });
  }

  ignore(data: any = {}) {
    let self = this;
    self.setBusy(true);

    self.updateForm(self.viewModelForm, {causality: 'IGNORED'});

    self.reportInstanceService.updateReportInstanceMedicationCausality(self.data.workFlowId, self.data.reportInstanceId, data.id, self.viewModelForm.value)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Causality set successfully", "Activity");
      self.initForm();
      self.loadData();
      self.selectedMedication = null;
    }, error => {
      this.handleError(error, "Error updating causality");
    });
  }

  private initForm(): void {
    let self = this;

    self.updateForm(self.viewModelForm, {question1: ''});
    self.updateForm(self.viewModelForm, {question2: ''});
    self.updateForm(self.viewModelForm, {question3: ''});
    self.updateForm(self.viewModelForm, {question4: ''});
    self.updateForm(self.viewModelForm, {question5: ''});
    self.updateForm(self.viewModelForm, {question6: ''});
    self.updateForm(self.viewModelForm, {question7: ''});
    self.updateForm(self.viewModelForm, {question8: ''});
    self.updateForm(self.viewModelForm, {question9: ''});
    self.updateForm(self.viewModelForm, {question10: ''});
    self.updateForm(self.viewModelForm, {causality: ''});
    self.updateForm(self.viewModelForm, {score: ''});

    self.calculation = '';
    self.score = null;
  }
}

export interface NaranjoPopupData {
  workFlowId: string;
  reportInstanceId: number;
  title: string;
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['legend-term', 'legend-description']);
  
  medicationGrid: GridModel<MedicationGridRecordModel> =
      new GridModel<MedicationGridRecordModel>
          (['medication', 'start-date', 'end-date', 'causality', 'actions']);
}

class GridRecordModel {
  term: string;
  description: string;
}

class MedicationGridRecordModel {
  id: number;
  reportInstanceId: number;
  reportInstanceMedicationGuid: string;
  medicationIdentifier: string;
  naranjoCausality: string;
  whoCausality: string;
  startDate: any;
  endDate: any;
}

const LEGEND_DATA: GridRecordModel[] = [
  {term: 'Definite', description: 'Score is greater than or equal to 9'},
  {term: 'Probable', description: 'Score is between 5 and 8'},
  {term: 'Possible', description: 'Score is between 1 and 4'},
  {term: 'Doubtful', description: 'Score is less than 1'},
];
