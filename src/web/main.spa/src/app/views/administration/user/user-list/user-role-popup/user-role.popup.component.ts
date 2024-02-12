import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, NgForm } from '@angular/forms';
import { concatMap, finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { RoleIdentifierModel, RoleIdentifierWrapperModel } from 'app/shared/models/user/role.identifier.model';
import { UserService } from 'app/shared/services/user.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { GridModel } from 'app/shared/models/grid.model';
import { UserRoleForUpdateModel } from 'app/shared/models/user/user-role-for-update.model';
import { forkJoin, from } from 'rxjs';

@Component({
  templateUrl: './user-role.popup.component.html',
  styles: [`
    .mat-column-role { flex: 0 0 75% !important; width: 75% !important; }
    .mat-column-actions { flex: 0 0 20% !important; width: 20% !important; }
  `],  
  animations: egretAnimations
})
export class UserRolePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  public viewModel: ViewModel = new ViewModel();

  roleList: RoleIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<UserRolePopupComponent>,
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
      roles: ['', Validators.required]
    })
    self.viewModel.mainGrid.setupBasic(null, null, null);
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.userId > 0) {
        self.loadData();
    }
  }

  saveUserRole() {
    let self = this;
    self.setBusy(true);

    self.userService.saveUserRole(self.data.userId, self.itemForm.get('roles').value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("User role added successfully", "Users");
        self.loadData();
    }, error => {
      self.handleError(error, "Error updating user role");
    });
  }

  deleteUserRole(role: string) {
    let self = this;
    self.setBusy(true);

    self.userService.deleteUserRole(self.data.userId, role)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("User role removed successfully", "Users");
        self.loadData();
    }, error => {
      self.handleError(error, "Error updating user role");
    });
  }  

  saveAllUserRole() {
    let self = this;

    from(self.viewModel.filteredRoles).pipe(
      concatMap(role => self.userService.saveUserRole(self.data.userId, role ))
    ).pipe(
      finalize(() => self.saveAllComplete()),
    ).subscribe(
        data => {
        },
        error => {
          this.handleError(error, "Error adding roles");
        });
  }

  cleanForm(form: NgForm) : void {
    form.resetForm();
  }

  private saveAllComplete(): void {
    let self = this;
    self.notify('Roles added successfully!', 'Users');
    self.loadData();
  }

  private loadData(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    requestArray.push(self.userService.getUserDetail(self.data.userId));
    requestArray.push(self.userService.getRoleList());

    forkJoin(requestArray)
      .subscribe(
        data => {
          let user = data[0] as any;
          self.viewModel.mainGrid.updateBasic(user.roles);

          let roles = data[1] as RoleIdentifierWrapperModel;
          var roleNames = roles.value.map(function(item) {
            return item['name'];
          });
          var filtered = roleNames.filter(
            function (e) {
              return this.indexOf(e) == -1;
            },
            user.roles
          );
          this.viewModel.filteredRoles = filtered;          

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });
  }  
}

export interface PopupData {
  userId: number;
  title: string;
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['role', 'actions']);
  filteredRoles: any;
}

class GridRecordModel {
  role: string;
}