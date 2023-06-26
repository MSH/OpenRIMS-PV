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
import { MetaPageService } from 'app/shared/services/meta-page.service';

@Component({
  templateUrl: './page-configure.popup.component.html',
  animations: egretAnimations
})
export class PageConfigurePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PageConfigurePopupData,
    public dialogRef: MatDialogRef<PageConfigurePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected metaPageService: MetaPageService,
    protected popupService: PopupService,
    protected accountService: AccountService,
  ) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      metaPageGuid: [''],
      pageName: ['', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]],
      pageDefinition: ['', [Validators.maxLength(250), Validators.pattern('[-a-zA-Z0-9 .,]*')]],
      breadcrumb: ['', [Validators.maxLength(250), Validators.pattern('[-a-zA-Z0-9 .,]*')]],
      visible: ['', Validators.required]
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.metaPageId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaPageService.getMetaPage(self.data.metaPageId, 'detail')
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, result);
      }, error => {
        this.handleError(error, "Error fetching meta page");
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.metaPageService.saveMetaPage(self.data.metaPageId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Meta page saved successfully", "Meta Pages");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error saving meta page");
    });
  }
}

export interface PageConfigurePopupData {
  metaPageId: number;
  title: string;
}