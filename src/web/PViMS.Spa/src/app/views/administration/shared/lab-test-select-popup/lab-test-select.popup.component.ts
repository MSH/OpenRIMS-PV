import { Component, OnInit, Inject, ViewEncapsulation, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { GridModel } from 'app/shared/models/grid.model';
import { finalize } from 'rxjs/operators';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { LabTestService } from 'app/shared/services/lab-test.service';

@Component({
  templateUrl: './lab-test-select.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class LabTestSelectPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {

  viewModel: ViewModel = new ViewModel();
  public itemForm: FormGroup;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: LabTestSelectPopupData,
    public dialogRef: MatDialogRef<LabTestSelectPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected labTestService: LabTestService,
  ) { 
    super(_router, _location, popupService, accountService);    
  }

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
      labTestName: ['']
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
  }  

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.labTestService.getLabTests(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.mainGrid.updateAdvance(result);
      }, error => {
        self.handleError(error, "Error fetching lab tests")
      });
  }

  selectLabTest(data: any) {
    this.dialogRef.close(data);    
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['lab-test', 'actions']);
}

class GridRecordModel {
  id: number;
  labTestName: string;
}

export interface LabTestSelectPopupData {
    title: string;
}