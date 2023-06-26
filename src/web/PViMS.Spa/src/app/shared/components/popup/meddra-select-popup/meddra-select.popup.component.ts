import { Component, OnInit, Inject, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { GridModel } from 'app/shared/models/grid.model';
import { MeddraTermService } from 'app/shared/services/meddra-term.service';
import { finalize } from 'rxjs/operators';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './meddra-select.popup.component.html',
  styles: [`
    .mat-column-meddra-term { flex: 0 0 75% !important; width: 75% !important; }
    .mat-column-actions { flex: 0 0 20% !important; width: 20% !important; }  
  `],   
  animations: egretAnimations
})
export class MeddraSelectPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {

  viewModel: ViewModel = new ViewModel();
  public itemForm: FormGroup;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MeddraSelectPopupData,
    public dialogRef: MatDialogRef<MeddraSelectPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected meddraTermService: MeddraTermService,
  ) { 
    super(_router, _location, popupService, accountService);    
  }

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
      searchOption: ['Term', Validators.required],
      termType: ['LLT', Validators.required],
      searchTerm: ['']
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

    self.meddraTermService.searchTerms(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.mainGrid.updateAdvance(result);
        self.viewModel.searched = true;
      }, error => {
        self.handleError(error, 'error fetching meddra terms');
      });
  }

  selectTerm(data: any) {
    this.dialogRef.close(data);    
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['meddra-term', 'actions']);
  
  searched: boolean = false;          
}

class GridRecordModel {
  id: number;
  medDraTerm: string;
}

export interface MeddraSelectPopupData {
    title: string;
}