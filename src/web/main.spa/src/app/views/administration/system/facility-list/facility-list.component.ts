import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
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
import { FacilityService } from 'app/shared/services/facility.service';
import { FacilityPopupComponent } from './facility-popup/facility.popup.component';
import { FacilityDeletePopupComponent } from './facility-delete-popup/facility-delete.popup.component';

@Component({
  templateUrl: './facility-list.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-facility-name { flex: 0 0 45% !important; width: 45% !important; }
    .mat-column-code { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-type { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }
  `],  
  animations: egretAnimations
})
export class FacilityListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected facilityService: FacilityService,
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

  formControl: FormControl = new FormControl();
  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    
    self.viewModelForm = self._formBuilder.group({
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

    self.facilityService.getFacilities(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.throwError(error, error.statusText);
        });
  }    

  openPopUp(data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Facility' : 'Update Facility';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FacilityPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { facilityId: isNew ? 0: data.id, title: title, payload: data }
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
    let title = 'Delete Facility';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FacilityDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { facilityId: data.id, title: title, payload: data }
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
          (['id', 'facility-name', 'code', 'type', 'actions']);
}

class GridRecordModel {
  id: number;
  facilityName: string;
  facilityType: string
  facilityCode: string
}
