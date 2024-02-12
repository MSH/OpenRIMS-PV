import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, NgForm } from '@angular/forms';
import { concatMap, finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { UserService } from 'app/shared/services/user.service';
import { FacilityService } from 'app/shared/services/facility.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { GridModel } from 'app/shared/models/grid.model';
import { forkJoin, from } from 'rxjs';
import { FacilityIdentifierModel } from 'app/shared/models/facility/facility.identifier.model';
import { FacilityDetailModel } from 'app/shared/models/facility/facility.detail.model';

@Component({
  templateUrl: './user-facility.popup.component.html',
  styles: [`
    .mat-column-role { flex: 0 0 75% !important; width: 75% !important; }
    .mat-column-actions { flex: 0 0 20% !important; width: 20% !important; }
  `],  
  animations: egretAnimations
})
export class UserFacilityPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  public viewModel: ViewModel = new ViewModel();

  facilityList: FacilityIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PopupData,
    public dialogRef: MatDialogRef<UserFacilityPopupComponent>,
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

    self.itemForm = this._formBuilder.group({
      facilities: ['', Validators.required]
    })
    self.viewModel.mainGrid.setupBasic(null, null, null);
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.userId > 0) {
        self.loadData();
    }
  }

  saveUserFacility() {
    let self = this;
    self.setBusy(true);

    self.userService.saveUserFacility(self.data.userId, +self.itemForm.get('facilities').value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("User facility added successfully", "Users");
        self.loadData();
    }, error => {
      self.handleError(error, "Error updating user facility");
    });
  }

  deleteUserFacility(facilityId: number) {
    let self = this;
    self.setBusy(true);

    self.userService.deleteUserFacility(self.data.userId, facilityId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("User facility removed successfully", "Users");
        self.loadData();
    }, error => {
      self.handleError(error, "Error updating user facility");
    });
  }  

  saveAllUserFacility() {
    let self = this;

    from(self.viewModel.filteredFacilities).pipe(
      concatMap(facility => this.processFilteredFacility(facility))
    ).pipe(
      finalize(() => self.saveAllComplete()),
    ).subscribe(
        data => {
        },
        error => {
          this.handleError(error, "Error adding facilities");
        });
  }

  cleanForm(form: NgForm) : void {
    form.resetForm();
  }

  private processFilteredFacility(facility: any): any {
    let self = this;
    return self.userService.saveUserFacility(self.data.userId, facility.id );
  }

  private saveAllComplete(): void {
    let self = this;
    self.notify('Facilities added successfully!', 'Users');
    self.loadData();
  }

  private loadData(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    requestArray.push(self.userService.getUserDetail(self.data.userId));
    requestArray.push(self.facilityService.getAllFacilities());

    forkJoin(requestArray)
      .subscribe(
        data => {
          let user = data[0] as any;
          self.viewModel.mainGrid.updateBasic(user.facilities);

          let facilityList = data[1] as FacilityDetailModel[];
          var filterFacilityIds = user.facilities.map(function(item) {
            return item['facilityId'];
          });
          var filteredFacilities = facilityList.filter(
            facility => !filterFacilityIds.includes(facility.id)
          );
          this.viewModel.filteredFacilities = filteredFacilities;          

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
          (['facility', 'actions']);
  filteredFacilities: any;
}

class GridRecordModel {
  id: number;
  facilityId: number;
  facilityName: string;
}