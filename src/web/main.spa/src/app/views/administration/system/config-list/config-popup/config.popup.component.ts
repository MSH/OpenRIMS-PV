import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';

import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ConfigService } from 'app/shared/services/config.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  selector: 'config-popup',
  templateUrl: './config.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class ConfigPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ConfigPopupData,
    public dialogRef: MatDialogRef<ConfigPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected configService: ConfigService,
    protected accountService: AccountService,
    protected popupService: PopupService,
  ) 
  { 
    super(_router, _location, popupService, accountService);    
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      configType: [this.data.payload.configValue || ''],
      configValue: [this.data.payload.configValue || '']
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.configId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.configService.getConfigIdentifier(self.data.configId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        self.handleError(error, "Error fetching config");
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.configService.saveConfig(self.data.configId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Configuration saved successfully", "Configurations");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error saving config");
    });
  }
}

export interface ConfigPopupData {
  configId: number;
  title: string;
  payload: any;
}