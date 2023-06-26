import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { PopupService } from 'app/shared/services/popup.service';
import { _routes } from 'app/config/routes';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { Form } from 'app/shared/indexed-db/appdb';

@Component({
  templateUrl: './attachment-view.popup.component.html',
  styleUrls: ['./attachment-view.popup.component.scss'], 
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations  
})
export class AttachmentViewPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ImageViewPopupData,
    public dialogRef: MatDialogRef<AttachmentViewPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected metaFormService: MetaFormService,
    protected accountService: AccountService,
    protected popupService: PopupService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  viewModel: ViewModel = new ViewModel();

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.formId > 0) {
      self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getForm(self.data.formId).then(result => {
      let form = result as Form;
      self.viewModel.attachment = self.data.index == 1 ? form.attachment : form.attachment_2;
      self.setBusy(false);
    }, error => {
      this.handleError(error, "Error loading image");
    });
  }

  deleteImage(): void {
    let self = this;

    self.metaFormService.deleteAttachment(self.data.formId, self.data.index).then(response =>
      {
        if (response) {
            self.notify('Image deleted successfully!', 'Form Saved');
            self.dialogRef.close("Removed");
        }
        else {
            self.showError('There was an error deleting the image, please try again !', 'Delete Image');
        }
      });   
  }
}

export interface ImageViewPopupData {
  formId: number;
  title: string;
  index: number;
}

class ViewModel {
  attachment: any;
}
