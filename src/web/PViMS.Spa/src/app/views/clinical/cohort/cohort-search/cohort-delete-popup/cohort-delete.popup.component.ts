import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './cohort-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class CohortDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CohortDeletePopupData,
    public dialogRef: MatDialogRef<CohortDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected cohortGroupService: CohortGroupService,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected formBuilder: FormBuilder,
  ) { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      cohortName: [this.data.payload.cohortName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.cohortGroupService.deleteCohortGroup(self.data.cohortGroupId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Cohort group deleted successfully", "Cohort Groups");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error deleting cohort group");
    });
  }
}

export interface CohortDeletePopupData {
  cohortGroupId: number;
  title: string;
  payload: any;
}