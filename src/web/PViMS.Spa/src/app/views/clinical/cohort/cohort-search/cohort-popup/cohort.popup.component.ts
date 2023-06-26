import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { ConditionIdentifierModel } from 'app/shared/models/condition/condition.identifier.model';
import { ConditionService } from 'app/shared/services/condition.service';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';

@Component({
  templateUrl: './cohort.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class CohortPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  conditionList: ConditionIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CohortPopupData,
    public dialogRef: MatDialogRef<CohortPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected conditionService: ConditionService,
    protected cohortGroupService: CohortGroupService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this._formBuilder.group({
      cohortName: [this.data.payload.cohortName || '', [Validators.required, Validators.maxLength(50), Validators.pattern("[a-zA-Z0-9 ']*")]],
      cohortCode: [this.data.payload.cohortCode || '', [Validators.required, Validators.maxLength(5), Validators.pattern("[-a-zA-Z0-9 ]*")]],
      conditionName: ['', Validators.required],
      startDate: ['', Validators.required],
      finishDate: ['']
    })
  }

  loadDropDowns(): void {
    let self = this;
    self.getConditionList();
  }

  getConditionList(): void {
    let self = this;

    self.conditionService.getAllConditions()
      .subscribe(result => {
        self.conditionList = result;
      }, error => {
        this.handleError(error, "Error fetching conditions");
      });
  }  

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.cohortGroupId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.cohortGroupService.getCohortGroupDetail(self.data.cohortGroupId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        console.log(result);
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        this.handleError(error, "Error fetching cohort group");
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.cohortGroupService.saveCohortGroup(self.data.cohortGroupId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Cohort group saved successfully", "Cohort Groups");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error saving cohort group");
    });
  }
}

export interface CohortPopupData {
  cohortGroupId: number;
  title: string;
  payload: any;
}