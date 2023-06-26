import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ConceptService } from 'app/shared/services/concept.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './medication-delete.popup.component.html',
  animations: egretAnimations
})
export class MedicationDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MedicationPopupData,
    public dialogRef: MatDialogRef<MedicationDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected conceptService: ConceptService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      conceptName: [this.data.payload.conceptName],
      productName: [this.data.payload.productName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.conceptService.deleteProduct(self.data.productId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Medication deleted successfully", "Medications");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        self.handleError(error, "Error deleting medication");
    });
  }
}

export interface MedicationPopupData {
  productId: number;
  title: string;
  payload: any;
}