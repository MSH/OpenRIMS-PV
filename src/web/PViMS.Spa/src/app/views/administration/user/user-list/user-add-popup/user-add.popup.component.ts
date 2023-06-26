import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { RoleIdentifierModel } from 'app/shared/models/user/role.identifier.model';
import { FacilityIdentifierModel } from 'app/shared/models/facility/facility.identifier.model';
import { UserService } from 'app/shared/services/user.service';
import { FacilityService } from 'app/shared/services/facility.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './user-add.popup.component.html',
  animations: egretAnimations
})
export class UserAddPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  roleList: RoleIdentifierModel[] = [];
  facilityList: FacilityIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: UserPopupData,
    public dialogRef: MatDialogRef<UserAddPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected userService: UserService,
    protected facilityService: FacilityService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this._formBuilder.group({
      firstName: [this.data.payload.firstName || '', [Validators.required, Validators.maxLength(30), Validators.pattern('[a-zA-Z ]*')]],
      lastName: [this.data.payload.lastName || '', [Validators.required, Validators.maxLength(30), Validators.pattern('[a-zA-Z ]*')]],
      userName: [this.data.payload.userName || '', [Validators.required, Validators.maxLength(30), Validators.pattern('[a-zA-Z0-9 ]*')]],
      email: ['', [Validators.required, Validators.maxLength(150), Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$")]],
      password: ['', [Validators.required, Validators.maxLength(100)]],
      confirmPassword: ['', [Validators.required, Validators.maxLength(100), confirmPasswordValidator]],
      roles: ['', Validators.required],
      facilities: ['', Validators.required],
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.userId > 0) {
        self.loadData();
    }
  }

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
    self.getRoleList();
  }

  getFacilityList(): void {
    let self = this;
    self.facilityService.getAllFacilities()
        .subscribe(result => {
            self.facilityList = result;
        }, error => {
          self.handleError(error, "Error fetching facilities");
        });
  }

  getRoleList(): void {
    let self = this;
    self.userService.getRoleList()
        .subscribe(result => {
          self.roleList = result.value;
        }, error => {
          self.handleError(error, "Error fetching roles");
        });
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
        self.handleError(error, "Error saving user");
    });
  }
}

export const confirmPasswordValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {

  if (!control.parent || !control)
      return null;

  const password = control.parent.get('password');
  const passwordConfirm = control.parent.get('confirmPassword');

  if (!password || !passwordConfirm)
      return null;

  if (passwordConfirm.value === '')
      return null;

  if (password.value === passwordConfirm.value)
      return null;

  return { 'passwordsNotMatching': true };
};

export interface UserPopupData {
  userId: number;
  title: string;
  payload: any;
}