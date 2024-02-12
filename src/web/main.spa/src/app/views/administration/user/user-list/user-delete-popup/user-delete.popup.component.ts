import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
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
  templateUrl: './user-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class UserDeletePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: UserPopupData,
    public dialogRef: MatDialogRef<UserDeletePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected accountService: AccountService,
    protected userService: UserService,
    protected popupService: PopupService,
  ) { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      userName: [this.data.payload.userName],
    })
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.userService.deleteUser(self.data.userId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("User deleted successfully", "Users");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        self.handleError(error, "Error deleting user");
    });
  }
}

export interface UserPopupData {
  userId: number;
  title: string;
  payload: any;
}