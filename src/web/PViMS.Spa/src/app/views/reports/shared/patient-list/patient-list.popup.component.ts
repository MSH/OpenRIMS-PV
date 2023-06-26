import { Component, OnInit, Inject, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { GridModel } from 'app/shared/models/grid.model';
import { PatientListModel } from 'app/shared/models/patient/patient-list.model';

@Component({
  templateUrl: './patient-list.popup.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-patient-name { flex: 0 0 40% !important; width: 40% !important; }
  `],  
  animations: egretAnimations
})
export class PatientListPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PatientListPopupData,
    public dialogRef: MatDialogRef<PatientListPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected changeDetectorRef: ChangeDetectorRef
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    let self = this;
    console.log(self.data.patients);
    self.viewModel.mainGrid.setupBasic(null, null, null);
    self.viewModel.mainGrid.updateBasic(self.data.patients);
    this.changeDetectorRef.detectChanges();
  }  
}

export interface PatientListPopupData {
  patients: PatientListModel[];
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['id', 'patient-name', 'facility-name']);
}

class GridRecordModel {
  patientId: number;
  fullName: string;
  facilityName: string; 
}