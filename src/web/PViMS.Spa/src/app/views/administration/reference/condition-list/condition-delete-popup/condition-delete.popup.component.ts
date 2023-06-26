import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { ConditionService } from 'app/shared/services/condition.service';

@Component({
  templateUrl: './condition-delete.popup.component.html',
  animations: egretAnimations
})
export class ConditionDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ConditionDeletePopupData,
    public dialogRef: MatDialogRef<ConditionDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected conditionService: ConditionService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = self._formBuilder.group({
      conditionName: [self.data.payload.conditionName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.conditionService.deleteCondition(self.data.conditionId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Condition deleted successfully", "Conditions");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error deleting condition");            
    });
  }
}

export interface ConditionDeletePopupData {
  conditionId: number;
  title: string;
  payload: any;
}