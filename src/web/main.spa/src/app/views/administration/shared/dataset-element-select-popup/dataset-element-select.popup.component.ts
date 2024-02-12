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
import { DatasetElementService } from 'app/shared/services/dataset-element.service';

@Component({
  templateUrl: './dataset-element-select.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class DatasetElementSelectPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {

  viewModel: ViewModel = new ViewModel();
  public itemForm: FormGroup;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MeddraSelectPopupData,
    public dialogRef: MatDialogRef<DatasetElementSelectPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected datasetElementService: DatasetElementService,
  ) { 
    super(_router, _location, popupService, accountService);    
  }

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
      elementName: ['']
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

    self.datasetElementService.getDatasetElements(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          self.handleError(error, "Error fetching dataset elements");
        });
  }

  selectElement(data: any) {
    this.dialogRef.close(data);    
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['element-name', 'actions']);
}

class GridRecordModel {
  id: number;
  datasetElementGuid: string;
  elementName: string;
  fieldTypeName: string;
}

export interface MeddraSelectPopupData {
    title: string;
}