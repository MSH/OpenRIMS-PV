import { Component, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { AccountService } from 'app/shared/services/account.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { UserService } from 'app/shared/services/user.service';
import { UserDetailModel } from 'app/shared/models/user/user.detail.model';
import { OrgUnitService } from 'app/shared/services/org-unit.service';
import { forkJoin } from 'rxjs';
import { OrgUnitIdentifierModel } from 'app/shared/models/org-unit/org-unit.identifier.model';
import { GridModel } from 'app/shared/models/grid.model';
import { UserFacilityModel } from 'app/shared/models/user/user-facility.model';

@Component({
  templateUrl: './user-profile.popup.component.html',
  animations: egretAnimations
})
export class UserProfilePopupComponent extends BasePopupComponent implements AfterViewInit {

  viewModel: ViewModel = new ViewModel();

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<UserProfilePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected userService: UserService,
    protected orgUnitService: OrgUnitService
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    const self = this;
    self.viewModel.facilitiesGrid.setupBasic(null, null, null);
  }

  ngAfterViewInit(): void {
    let self = this;
    self.loadData();
  }

  selectOrgUnit(orgUnit: OrgUnitIdentifierModel): void {
    let self = this;

    self.viewModel.selectedOrgUnit = orgUnit;
    self.viewModel.filteredFacilities = self.viewModel.user.facilities.filter(f => f.orgUnitName == orgUnit.orgUnitName);

    self.viewModel.facilitiesGrid.updateBasic(self.viewModel.filteredFacilities);
  }

  private loadData(): void {
    let self = this;

    const requestArray = [];

    requestArray.push(self.userService.getUserDetail(+self.accountService.getUniquename()));
    requestArray.push(self.orgUnitService.getAllOrgUnits());

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.viewModel.user = data[0] as UserDetailModel;
          self.viewModel.orgUnitList = data[1] as OrgUnitIdentifierModel[];

          if(self.viewModel.orgUnitList.length > 0) {
            self.selectOrgUnit(self.viewModel.orgUnitList[0]);
          }

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        });    
  }  
}

export interface PopupData {
  title: string;
}

class ViewModel {
  user: UserDetailModel;
  orgUnitList: OrgUnitIdentifierModel[] = [];
  selectedOrgUnit: OrgUnitIdentifierModel;

  filteredFacilities: UserFacilityModel[] = [];

  facilitiesGrid: GridModel<UserFacilityModel> =
      new GridModel<UserFacilityModel>
          (['facility-name']);  
}