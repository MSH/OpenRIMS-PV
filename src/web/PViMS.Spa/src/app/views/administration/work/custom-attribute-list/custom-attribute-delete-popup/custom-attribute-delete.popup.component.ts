import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Router } from '@angular/router';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { CustomAttributeService } from 'app/shared/services/custom-attribute.service';

@Component({
  templateUrl: './custom-attribute-delete.popup.component.html',
  animations: egretAnimations
})
export class CustomAttributeDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<CustomAttributeDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected customAttributeService: CustomAttributeService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      attributeKey: [this.data.attributeKey],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.customAttributeService.deleteCustomAttribute(self.data.customAttributeId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Custom attribute deleted successfully", "Attributes");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error deleting attribute");      
    });
  }
}

export interface PopupData {
  customAttributeId: number;
  attributeKey: string;
  title: string;
}