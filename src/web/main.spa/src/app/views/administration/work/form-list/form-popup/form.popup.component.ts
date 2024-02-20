import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { CohortGroupIdentifierModel } from 'app/shared/models/cohort/cohort-group.identifier.model';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { forkJoin } from 'rxjs';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './form.popup.component.html',
  animations: egretAnimations
})
export class FormPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  viewModel: ViewModel = new ViewModel();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FormPopupData,
    public dialogRef: MatDialogRef<FormPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected cohortGroupService: CohortGroupService,
    protected metaFormService: MetaFormService,
    protected accountService: AccountService,
    protected popupService: PopupService,
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();    

    self.itemForm = this._formBuilder.group({
      cohortGroupId: ['', Validators.required],
      cohortGroup: [''],
      formName: ['', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]],
      actionName: ['', [Validators.required, Validators.maxLength(20), Validators.pattern('[a-zA-Z0-9 ]*')]],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.formId > 0) {
        self.fetchView();
    }
  }

  fetchView(): void {
    let self = this;
    self.updateForm(self.itemForm, self.data.payload);
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.metaFormService.saveMetaForm(self.data.formId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Meta form saved successfully", "Meta Forms");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error saving meta form");
    });
  }

  private loadDropDowns(): void {
    const self = this;
    self.setBusy(true);

    const requestArray = [];
    requestArray.push(self.cohortGroupService.getAllCohortGroups());

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.viewModel.cohortLists = data[0] as CohortGroupIdentifierModel[];
          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });

  }  
}

export interface FormPopupData {
  formId: number;
  title: string;
  payload: any;
}

class ViewModel {
  cohortLists: CohortGroupIdentifierModel[] = [];
}