import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { LabTestService } from 'app/shared/services/lab-test.service';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';

@Component({
  templateUrl: './lab.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class LabPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: LabTestPopupData,
    public dialogRef: MatDialogRef<LabPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected labTestService: LabTestService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      labTestName: [this.data.payload.labTestName || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]],
      active: ['', Validators.required]
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.labTestId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.labTestService.getLabTestIdentifier(self.data.labTestId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        this.handleError(error, "Error fetching test or procedure");
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.labTestService.saveLabTest(self.data.labTestId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Test and procedure saved successfully", "Tests and Procedures");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error saving test or procedure");
    });
  }
}

export interface LabTestPopupData {
  labTestId: number;
  title: string;
  payload: any;
}