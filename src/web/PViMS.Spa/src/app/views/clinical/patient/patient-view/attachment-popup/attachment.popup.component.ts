import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PopupService } from 'app/shared/services/popup.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { PatientService } from 'app/shared/services/patient.service';
import { finalize } from 'rxjs/operators';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { egretAnimations } from 'app/shared/animations/egret-animations';

@Component({
  templateUrl: './attachment.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class AttachmentPopupComponent extends BasePopupComponent implements OnInit {
  
  public progress: number;
  public itemForm: FormGroup;

  fileToUpload: File = null;
  
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AttachmentPopupData,
    public dialogRef: MatDialogRef<AttachmentPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected patientService: PatientService
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
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.patientService.saveAttachment(self.data.patientId, self.fileToUpload, self.itemForm.get('description').value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Attachment successfully uploaded!", "Success");
        self.dialogRef.close(self.fileToUpload.name);
      }, error => {
        if(error.status == 400) {
          self.showInfo(error.error.message[0], error.statusText);
        } else {
          self.throwError(error, error.statusText);
        }
    });
  }  
}

export interface AttachmentPopupData {
  patientId: number;
  title: string;
}