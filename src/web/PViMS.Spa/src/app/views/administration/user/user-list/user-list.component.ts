import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Subscription } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { UserService } from 'app/shared/services/user.service';
import { UserDeletePopupComponent } from './user-delete-popup/user-delete.popup.component';
import { UserAddPopupComponent } from './user-add-popup/user-add.popup.component';
import { UserUpdatePopupComponent } from './user-update-popup/user-update.popup.component';
import { PasswordResetPopupComponent } from './password-reset-popup/password-reset.popup.component';
import { UserRolePopupComponent } from './user-role-popup/user-role.popup.component';
import { UserFacilityPopupComponent } from './user-facility-popup/user-facility.popup.component';

@Component({
  templateUrl: './user-list.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-actions { flex: 0 0 15% !important; width: 15% !important; }
  `],  
  animations: egretAnimations
})
export class UserListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected userService: UserService,
    protected dialog: MatDialog,
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          this.setupTable();
      }
    });    
  }

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    
    self.viewModelForm = self._formBuilder.group({
      userName: [this.viewModel.userName || ''],
    });     
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
      null, null, self.mainGridPaginator)
      .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  } 

  setupTable() {
  };

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.userService.getUsers(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.mainGrid.updateAdvance(result);
      }, error => {
        self.handleError(error, 'Error fetching users');
      });
  }    

  openAddPopUp(data: any = {}) {
    let self = this;
    let title = 'Add User';
    let dialogRef: MatDialogRef<any> = self.dialog.open(UserAddPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { userId: 0, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();        
      })
  } 

  openUpdatePopUp(data: any = {}) {
    let self = this;
    let title = 'Update User';
    let dialogRef: MatDialogRef<any> = self.dialog.open(UserUpdatePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { userId: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();        
      })
  } 
  
  openDeletePopUp(data: any = {}) {
    let self = this;
    let title = 'Delete User';
    let dialogRef: MatDialogRef<any> = self.dialog.open(UserDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { userId: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  } 

  openResetPopUp(data: any = {}) {
    let self = this;
    let title = 'Reset Password';
    let dialogRef: MatDialogRef<any> = self.dialog.open(PasswordResetPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { userId: data.id, title: title, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }

  openRolePopUp(data: any = {}) {
    let self = this;
    let title = 'Manage Roles';
    let dialogRef: MatDialogRef<any> = self.dialog.open(UserRolePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { userId: data.id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }

  openFacilityPopUp(data: any = {}) {
    let self = this;
    let title = 'Manage Facilities';
    let dialogRef: MatDialogRef<any> = self.dialog.open(UserFacilityPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { userId: data.id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['id', 'user-name', 'first-name', 'last-name', 'actions']);
  
  userName: string;
}

class GridRecordModel {
  id: number;
  userName: string;
  firstName: string;
  lastName: string;
}
