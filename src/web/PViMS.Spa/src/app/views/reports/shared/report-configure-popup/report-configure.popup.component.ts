import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { MetaReportService } from 'app/shared/services/meta-report.service';
import { MetaTableDetailModel } from 'app/shared/models/meta/meta-table.detail.model';
import { MetaService } from 'app/shared/services/meta.service';

@Component({
  templateUrl: './report-configure.popup.component.html',
  animations: egretAnimations
})
export class ReportConfigurePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ReportConfigurePopupData,
    public dialogRef: MatDialogRef<ReportConfigurePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected metaReportService: MetaReportService,
    protected metaService: MetaService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  metaTableList: MetaTableDetailModel[] = [];

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this._formBuilder.group({
      metaReportGuid: [''],
      reportName: ['', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]],
      reportDefinition: ['', [Validators.maxLength(250), Validators.pattern('[-a-zA-Z0-9 .,]*')]],
      reportStatus: ['', Validators.required],
      reportType: [''],
      coreEntity: ['']
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.metaReportId > 0) {
        self.loadData();
    }
  }

  loadDropDowns(): void {
    let self = this;
    self.getMetaTableList();
  }

  getMetaTableList(): void {
    let self = this;

    self.metaService.getAllMetaTables()
      .subscribe(result => {
        self.metaTableList = result;
      }, error => {
          self.handleError(error, "Error fetching meta tables");
      });
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaReportService.getMetaReportByDetail(self.data.metaReportId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, result);
        console.log(result);
      }, error => {
        this.handleError(error, "Error fetching meta report");
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.metaReportService.saveMetaReport(self.data.metaReportId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Meta report saved successfully", "Meta Reports");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error saving meta report");
    });
  }
}

export interface ReportConfigurePopupData {
  metaReportId: number;
  title: string;
}