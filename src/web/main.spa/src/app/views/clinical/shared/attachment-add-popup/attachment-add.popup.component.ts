import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PopupService } from 'app/shared/services/popup.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';

@Component({
  templateUrl: './attachment-add.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class AttachmentAddPopupComponent extends BasePopupComponent implements OnInit {
  
  public progress: number;
  public itemForm: FormGroup;

  fileToUpload: File = null;
  fileSizeLarge: boolean = false;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AttachmentAddPopupData,
    public dialogRef: MatDialogRef<AttachmentAddPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
      description: ['', [Validators.maxLength(100), Validators.pattern('[a-zA-Z0-9 ]*')]],
    })
  }

  handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
    this.fileSizeLarge = false;
    if(this.fileToUpload.size > 2560000) {
      this.CLog('file too large');
      this.fileSizeLarge = true;
    }
  }

  addAttachment() {
    let self = this;
    self.setBusy(true);

    const attachmentModel: FormAttachmentModel = {
      description: self.itemForm.get('description').value,
      file: self.fileToUpload
    };
    
    self.notify("Attachment successfully added!", "Success");
    self.dialogRef.close(attachmentModel);
  }  
}

export interface AttachmentAddPopupData {
  title: string;
}