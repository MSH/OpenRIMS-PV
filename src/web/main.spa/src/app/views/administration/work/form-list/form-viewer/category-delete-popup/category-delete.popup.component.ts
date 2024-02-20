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
import { MetaFormService } from 'app/shared/services/meta-form.service';

@Component({
  templateUrl: './category-delete.popup.component.html',
  animations: egretAnimations
})
export class CategoryDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CategoryPopupData,
    public dialogRef: MatDialogRef<CategoryDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected metaFormService: MetaFormService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      name: [this.data.payload.categoryName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.metaFormService.deleteMetaCategory(self.data.metaFormId, self.data.metaCategoryId)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Category deleted successfully", "Delete Category");
      this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error deleting category");
    });
  }
}

export interface CategoryPopupData {
  metaFormId: number;
  metaCategoryId: number;
  title: string;
  payload: any;
}