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
import { MetaFormService } from 'app/shared/services/meta-form.service';

@Component({
  templateUrl: './category-configure.popup.component.html',
  animations: egretAnimations
})
export class CategoryConfigurePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CategoryConfigurePopupData,
    public dialogRef: MatDialogRef<CategoryConfigurePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected metaFormService: MetaFormService,
    protected popupService: PopupService,
    protected accountService: AccountService) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  viewModel: ViewModel = new ViewModel();

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      metaFormCategoryGuid: [''],
      metaTableName: [''],
      metaTableId: ['', Validators.required],
      categoryName: ['', [Validators.required, Validators.maxLength(150), Validators.pattern('[-a-zA-Z0-9 .,()*]*')]],
      help: ['', [Validators.maxLength(500), Validators.pattern('[-a-zA-Z0-9 .,()*]*')]],
      icon: ['', Validators.required],
    })

  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.metaCategoryId > 0) {
        self.fetchView();
    }
  }

  submit() {
    let self = this;

    if (self.itemForm.valid) {
      self.setBusy(true);

      self.metaFormService.saveMetaCategory(self.data.metaFormId, self.data.metaCategoryId, self.itemForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify("Meta category saved successfully", "Meta Forms");
          this.dialogRef.close(this.itemForm.value);
      }, error => {
        this.handleError(error, "Error saving meta category");
      });
    }
  }  

  private fetchView(): void {
    let self = this;
    self.updateForm(self.itemForm, self.data.payload);    
  }  
}

class ViewModel {
  selectedIcon: string = '';
}

export interface CategoryConfigurePopupData {
  metaFormId: number;
  metaCategoryId: number;
  title: string;
  payload: any;
}