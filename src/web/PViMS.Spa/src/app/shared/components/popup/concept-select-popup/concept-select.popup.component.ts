import { Component, OnInit, Inject, ViewChild, AfterViewInit } from '@angular/core';
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
import { ConceptService } from 'app/shared/services/concept.service';

@Component({
  templateUrl: './concept-select.popup.component.html',
  styles: [`
    .mat-column-display-name { flex: 0 0 70% !important; width: 70% !important; }
  `],   
  animations: egretAnimations
})
export class ConceptSelectPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {

  viewModel: ViewModel = new ViewModel();
  public itemForm: FormGroup;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ConceptSelectPopupData,
    public dialogRef: MatDialogRef<ConceptSelectPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected conceptService: ConceptService
  ) { 
    super(_router, _location, popupService, accountService);    
  }

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
      searchOption: ['Concept', Validators.required],
      searchTerm: [''],
      active: ['Yes']      
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

    if(self.itemForm.value.searchOption == 'Concept')
    {
      self.conceptService.searchConcepts(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.mainGrid.updateAdvance(result);
      }, error => {
        this.handleError(error, "Error fetching concepts");
      });
    }
    else {
      self.conceptService.searchProducts(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.mainGrid.updateAdvance(result);
      }, error => {
        this.handleError(error, "Error fetching products");
      });
    }
  }

  select(data: any) {
    data.source = this.itemForm.value.searchOption;
    this.dialogRef.close(data);    
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['display-name', 'actions']);
}

class GridRecordModel {
  id: number;
  medDraTerm: string;
}

export interface ConceptSelectPopupData {
    title: string;
    productOnly: boolean;
}