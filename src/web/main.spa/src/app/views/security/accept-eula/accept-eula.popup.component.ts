import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { UserService } from 'app/shared/services/user.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './accept-eula.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class AcceptEulaPopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EulaPopupData,
    public dialogRef: MatDialogRef<AcceptEulaPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected userService: UserService,
  ) { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.userService.acceptEula(+this.accountService.getUniquename())
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Eula accepted successfully", "User");
        this.dialogRef.close(true);
    }, error => {
      self.handleError(error, "Error accepting EULA");
    });
  }
}

export interface EulaPopupData {
  title: string;
}