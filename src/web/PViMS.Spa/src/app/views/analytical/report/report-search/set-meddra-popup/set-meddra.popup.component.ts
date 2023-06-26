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
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { MeddraTermIdentifierModel } from 'app/shared/models/terminology/meddra-term.identifier.model';

@Component({
  templateUrl: './set-meddra.popup.component.html',
  styles: [`
    .mat-column-meddra-term { flex: 0 0 75% !important; width: 75% !important; }
    .mat-column-actions { flex: 0 0 20% !important; width: 20% !important; }  
  `],   
  animations: egretAnimations
})
export class SetMeddraPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {

  viewModel: ViewModel = new ViewModel();
  public itemForm: FormGroup;
  
  termSOCList: MeddraTermIdentifierModel[];
  termHLGTList: MeddraTermIdentifierModel[];
  termHLTList: MeddraTermIdentifierModel[];
  termPTList: MeddraTermIdentifierModel[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: SetMeddraPopupData,
    public dialogRef: MatDialogRef<SetMeddraPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected meddraTermService: MeddraTermService,
    protected reportInstanceService: ReportInstanceService
  ) { 
    super(_router, _location, popupService, accountService);    
  }

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
      sourceIdentifier: [''],
      searchOption: ['Term', Validators.required],
      termType: ['LLT'],
      searchTerm: ['', Validators.maxLength(100)],
      searchCode: ['', Validators.maxLength(10)],

      termSOC: [''],
      termHLGT: [''],
      termHLT: [''],
      termPT: [''],

      terminologyMedDraId: []
    })

    this.loadDropDowns();
  }

  loadDropDowns(): void {
    let self = this;
    self.getSOCTermList();
  }    

  getSOCTermList(): void {
    let self = this;

    self.updateForm(self.itemForm, {termType: 'SOC'});
    self.updateForm(self.itemForm, {searchTerm: ''});

    self.meddraTermService.getAllSOCTerms(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
        .subscribe(result => {
          self.termSOCList = result;
          self.updateForm(self.itemForm, {termType: 'LLT'});
        }, error => {
            self.handleError(error, "Error fetching SOC list");
        });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });

    self.loadData();
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.reportInstanceService.getReportInstanceDetail(self.data.workFlowId, self.data.reportInstanceId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, result);
      }, error => {
        this.handleError(error, "Error fetching report instance");
      });
  }

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.viewModel.searched = true;

    switch (self.itemForm.value.searchOption) {
      case "Common":
        self.meddraTermService.getCommonTerms(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error loading common terms");
        });

        break;

      case "Term":
        self.meddraTermService.searchTerms(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching by meddra term");
        });

        break;

      case "Code":
        self.meddraTermService.searchTerms(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching by meddra code");
        });

        break;

      case "List":
        self.meddraTermService.searchTerms(self.viewModel.mainGrid.customFilterModel(self.itemForm.value))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching by meddra list");
        });

        break;
    }
  }

  loadHLGTList(e) {
    let self = this;
    self.setBusy(true);
    self.meddraTermService.getMeddraTermDetail(e.source.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.termHLGTList = result.children;
      }, error => {
        this.handleError(error, "Error loading SOC meddra term");
      });
  }

  loadHLTList(e) {
    let self = this;
    self.setBusy(true);
    self.meddraTermService.getMeddraTermDetail(e.source.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.termHLTList = result.children;
      }, error => {
        this.handleError(error, "Error loading HLGT meddra term");
      });
  }

  loadPTList(e) {
    let self = this;
    self.setBusy(true);
    self.meddraTermService.getMeddraTermDetail(e.source.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.termPTList = result.children;
      }, error => {
        this.handleError(error, "Error loading HLT meddra term");
      });
  }

  selectTerm(data: any) {
    let self = this;
    self.setBusy(true);

    self.updateForm(self.itemForm, {terminologyMedDraId: data.id});

    self.reportInstanceService.updateTerminology(self.data.workFlowId, self.data.reportInstanceId, self.itemForm.value)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Terminology set successfully", "Activity");
      this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error updating status");
    });
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['meddra-term', 'actions']);
  
  terminologyMedDraId: number;
  searched: boolean = false;
}

class GridRecordModel {
  id: number;
  medDraTerm: string;
}

export interface SetMeddraPopupData {
  workFlowId: string;
  reportInstanceId: number;
  title: string;
}