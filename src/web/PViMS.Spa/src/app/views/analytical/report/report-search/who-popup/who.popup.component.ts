import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTabGroup } from '@angular/material/tabs';
import { FormBuilder, FormGroup } from '@angular/forms';
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
  templateUrl: './who.popup.component.html',
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
export class WhoPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
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
    @Inject(MAT_DIALOG_DATA) public data: WhoPopupData,
    public dialogRef: MatDialogRef<WhoPopupComponent>,
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
  legendDataSource = LEGEND_DATA;
  selectedMedication: ReportInstanceMedicationDetailModel = null;
  selectedStatus: string = 'Certain';

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
      question11: [''],
      question12: [''],
      question13: [''],
      question14: [''],
      question15: [''],
      question16: [''],
      question17: [''],
      question18: [''],
      causalityConfigType: ['3'],
      causality: ['']
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
        console.log(result);
        self.updateForm(self.viewModelForm, result);
        self.updateForm(self.viewModelForm, {meddraTerm: result.terminologyMedDra.medDraTerm});

        self.viewModel.medicationGrid.updateBasic(result.medications);
      }, error => {
        this.handleError(error, "Error loading report instance");
      });
  }

  resetSelection(selectedStatus: string) {
    let self = this;
    self.selectedStatus = selectedStatus;
    self.calculation = '';
  }

  handleProbable(e) {
    let self = this;

    if (self.viewModelForm.get('question1').value == '' 
        || self.viewModelForm.get('question2').value == ''  
        || self.viewModelForm.get('question3').value == '' 
        || self.viewModelForm.get('question4').value == ''  
        || self.viewModelForm.get('question5').value == '') {
          self.calculation = '';
      return;
    }

    if (self.viewModelForm.get('question1').value == 'Yes' 
        && self.viewModelForm.get('question2').value == 'Yes'  
        && self.viewModelForm.get('question3').value == 'Yes' 
        && self.viewModelForm.get('question4').value == 'Yes'  
        && self.viewModelForm.get('question5').value == 'Yes') {
          self.calculation = 'Certain';
      return;
    }

    self.selectedStatus = 'Probable/Likely';
    self.calculation = '';
  }  

  handlePossible(e) {
    let self = this;

    if (self.viewModelForm.get('question6').value == '' 
        || self.viewModelForm.get('question7').value == ''  
        || self.viewModelForm.get('question8').value == '' 
        || self.viewModelForm.get('question9').value == '') {
          self.calculation = '';
      return;
    }

    if (self.viewModelForm.get('question6').value == 'Yes' 
        && self.viewModelForm.get('question7').value == 'Yes'  
        && self.viewModelForm.get('question8').value == 'Yes' 
        && self.viewModelForm.get('question9').value == 'Yes') {
          self.calculation = 'Probable';
      return;
    }

    self.selectedStatus = 'Possible';
    self.calculation = '';
  }  

  handleUnlikely(e) {
    let self = this;

    if (self.viewModelForm.get('question10').value == '' 
        || self.viewModelForm.get('question11').value == ''  
        || self.viewModelForm.get('question12').value == '') {
          self.calculation = '';
      return;
    }

    if (self.viewModelForm.get('question10').value == 'Yes' 
        && self.viewModelForm.get('question11').value == 'Yes'  
        && self.viewModelForm.get('question12').value == 'Yes') {
          self.calculation = 'Possible';
      return;
    }

    self.selectedStatus = 'Unlikely';
    self.calculation = '';
  }  

  handleConditional(e) {
    let self = this;

    if (self.viewModelForm.get('question13').value == '' 
        || self.viewModelForm.get('question14').value == '') {
          self.calculation = '';
      return;
    }

    if (self.viewModelForm.get('question13').value == 'Yes' 
        && self.viewModelForm.get('question14').value == 'Yes') {
          self.calculation = 'Unlikely';
      return;
    }

    self.selectedStatus = 'Conditional/Unclassified';
    self.calculation = '';
  }  

  handleUnassessable(e) {
    let self = this;

    if (self.viewModelForm.get('question15').value == '' 
        || self.viewModelForm.get('question16').value == '') {
          self.calculation = '';
      return;
    }

    if (self.viewModelForm.get('question15').value == 'Yes' 
        && self.viewModelForm.get('question16').value == 'Yes') {
          self.calculation = 'Conditional';
      return;
    }

    self.selectedStatus = 'Unassessable/Unclassified';
    self.calculation = '';
  }  

  handleFinal(e) {
    let self = this;

    if (self.viewModelForm.get('question17').value == '' 
        || self.viewModelForm.get('question18').value == '') {
          self.calculation = '';
      return;
    }

    if (self.viewModelForm.get('question17').value == 'Yes' 
        && self.viewModelForm.get('question18').value == 'Yes') {
          self.calculation = 'Unassessable';
      return;
    }

    self.calculation = '';
  } 

  openCausality(data: any = {}) {
    this.selectedMedication = data;
    this.mainTabGroup.selectedIndex = 4;
  }

  setCausality() {
    let self = this;
    self.setBusy(true);

    self.updateForm(self.viewModelForm, {causality: self.calculation});

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
    self.updateForm(self.viewModelForm, {question11: ''});
    self.updateForm(self.viewModelForm, {question12: ''});
    self.updateForm(self.viewModelForm, {question13: ''});
    self.updateForm(self.viewModelForm, {question14: ''});
    self.updateForm(self.viewModelForm, {question15: ''});
    self.updateForm(self.viewModelForm, {question16: ''});
    self.updateForm(self.viewModelForm, {question17: ''});
    self.updateForm(self.viewModelForm, {question18: ''});
    self.updateForm(self.viewModelForm, {causality: ''});

    self.calculation = '';
    self.selectedStatus = 'Certain'    
  }  
}

export interface WhoPopupData {
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
  {term: 'Certain', description: 'Item 1 to 5 answered in the affirmative'},
  {term: 'Probable/Likely', description: 'Item 6 to 9 answered in the affirmative'},
  {term: 'Possible', description: 'Item 10 to 12 answered in the affirmative'},
  {term: 'Unlikely', description: 'Item 13 to 14 answered in the affirmative'},
  {term: 'Conditional', description: 'Item 15 to 16 answered in the affirmative'},
  {term: 'Unassessable', description: 'Item 17 to 18 answered in the affirmative'},
];
