import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { LabTestService } from 'app/shared/services/lab-test.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './form.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: LabResultPopupData,
    public dialogRef: MatDialogRef<FormPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected labTestService: LabTestService,
    protected accountService: AccountService,
    protected popupService: PopupService,
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      labResultName: [this.data.payload.labResultName || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]],
      active: ['', Validators.required]
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.labResultId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.labTestService.getLabResultIdentifier(self.data.labResultId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        self.throwError(error, error.statusText);
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.labTestService.saveLabResult(self.data.labResultId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Test result saved successfully", "Tests Results");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        if(error.status == 400) {
          self.showInfo(error.error.message[0], error.statusText);
        } else {
          self.throwError(error, error.statusText);
        }
    });
  }
}

export interface LabResultPopupData {
  labResultId: number;
  title: string;
  payload: any;
}