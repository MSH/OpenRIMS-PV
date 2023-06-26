import { Component, OnInit, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { PopupService } from 'app/shared/services/popup.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { FormGroup, FormBuilder } from '@angular/forms';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MeddraTermService } from 'app/shared/services/meddra-term.service';

@Component({
  templateUrl: './import-meddra.popup.component.html',
  animations: egretAnimations
})
export class ImportMeddraPopupComponent extends BasePopupComponent implements OnInit {
  
  public progress: number;
  public itemForm: FormGroup;

  fileToUpload: File = null;
  
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ImportMeddraPopupData,
    public dialogRef: MatDialogRef<ImportMeddraPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected meddraTermService: MeddraTermService
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    this.itemForm = this._formBuilder.group({
    })
  }

  handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.meddraTermService.saveMedDraFile(self.fileToUpload)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Meddra terms successfully uploaded!", "Success");
        self.dialogRef.close(self.fileToUpload.name);
      }, error => {
        self.handleError(error, "Error saving MedDRA file for import");
    });
  }
}

export interface ImportMeddraPopupData {
  title: string;
}