import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { MetaReportService } from 'app/shared/services/meta-report.service';

@Component({
  templateUrl: './generic-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class GenericDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: GenericPopupData,
    public dialogRef: MatDialogRef<GenericDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected metaReportService: MetaReportService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      name: [this.data.name],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    switch (self.data.type) {
      case "MetaReport":
        self.metaReportService.deleteMetaReport(self.data.id)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify(self.data.type + " deleted successfully", self.data.type);
          this.dialogRef.close(this.itemForm.value);
        }, error => {
          this.handleError(error, "Error deleting " + self.data.type);
        });
  
        break;
    }    
  }
}

export interface GenericPopupData {
  parentId: number;
  id: number;
  title: string;
  type: 'MetaReport';
  name: string;
}