import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GridModel } from 'app/shared/models/grid.model';
import { ReportInstanceMedicationDetailModel } from 'app/shared/models/report-instance/report-instance-medication.detail.model';

@Component({
  styles: [`
    .mat-column-medication { flex: 0 0 50% !important; width: 50% !important; }
    .mat-column-naranjo-causality { flex: 0 0 20% !important; width: 20% !important; }
    .mat-column-who-causality { flex: 0 0 20% !important; width: 20% !important; }
    .tag-red {
      padding-right: 8px;
      padding-left: 8px;
      border-radius: 12px;
      line-height: 24px;
      box-sizing: content-box;
      background: red;
      color: white;
    
      & > div {
        display: inline-block;
      }
    }
    .tag-black {
      padding-right: 8px;
      padding-left: 8px;
      border-radius: 12px;
      line-height: 24px;
      box-sizing: content-box;
      background: black;
      color: white;
    
      & > div {
        display: inline-block;
      }
    }
  `],  
  templateUrl: './medication-list.popup.component.html'
})
export class MedicationListPopupComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MedicationListPopupData,
    public dialogRef: MatDialogRef<MedicationListPopupComponent>,
  ) { }

  ngOnInit(): void {
    const self = this;
    self.viewModel.medicationGrid.setupBasic(null, null, null);
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.medicationGrid.updateBasic(self.data.medications);
  }  
}

export interface MedicationListPopupData {
  medications: ReportInstanceMedicationDetailModel[];
  title: string;
}

class ViewModel {
  medicationGrid: GridModel<ReportInstanceMedicationDetailModel> =
      new GridModel<ReportInstanceMedicationDetailModel>
          (['medication', 'naranjo-causality', 'who-causality']);
}