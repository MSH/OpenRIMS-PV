import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { UserService } from 'app/shared/services/user.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './user-update.popup.component.html',
  animations: egretAnimations
})
export class UserUpdatePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: UserPopupData,
    public dialogRef: MatDialogRef<UserUpdatePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected userService: UserService
  ) { 
    super(_router, _location, popupService, accountService);        
  }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this._formBuilder.group({
      firstName: [this.data.payload.firstName || '', [Validators.required, Validators.maxLength(30), Validators.pattern('[a-zA-Z ]*')]],
      lastName: [this.data.payload.lastName || '', [Validators.required, Validators.maxLength(30), Validators.pattern('[a-zA-Z ]*')]],
      userName: [this.data.payload.userName || '', [Validators.required, Validators.maxLength(30), Validators.pattern('[a-zA-Z0-9 ]*')]],
      email: ['', [Validators.required, Validators.maxLength(150), Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]],
      allowDatasetDownload: ['', Validators.required],
      active: ['', Validators.required]
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.userId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.userService.getUserDetail(self.data.userId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, (self.data.payload = result));
      }, error => {
        self.handleError(error, "Error fetching user");
      });
  }  

  submit() {
    let self = this;
    self.setBusy(true);

    self.userService.saveUser(self.data.userId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("User saved successfully", "Users");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      self.handleError(error, "Error updating user");
    });
  }
}

export interface UserPopupData {
  userId: number;
  title: string;
  payload: any;
}