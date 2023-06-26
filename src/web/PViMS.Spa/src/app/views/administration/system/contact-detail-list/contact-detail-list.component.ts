import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver } from '@angular/flex-layout';
import { GridModel } from 'app/shared/models/grid.model';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ContactDetailService } from 'app/shared/services/contact-detail.service';
import { ContactDetailPopupComponent } from './contact-detail-popup/contact-detail.popup.component';

@Component({
  templateUrl: './contact-detail-list.component.html',
  styles: [`
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],  
  animations: egretAnimations
})
export class ContactDetailListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected contactDetailService: ContactDetailService,
    protected dialog: MatDialog,    
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);
  }

  viewModel: ViewModel = new ViewModel();

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, null)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  } 

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.contactDetailService.getContactList()
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          self.handleError(error, "Error fetching contacts");
        });
  }

  openPopUp(data: any = {}) {
    let self = this;
    let title = 'Update Detail';
    let dialogRef: MatDialogRef<any> = self.dialog.open(ContactDetailPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { contactId: data.id, title: title, payload: data }
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
          (['contact-type', 'organisation-name', 'contact-first-name', 'contact-last-name', 'contact-number', 'contact-email', 'actions']);
}

class GridRecordModel {
  id: number;
  contactType: string;
  organisationName: string;
  contactFirstName: string;
  contactLastName: string;
  contactNumber: string;
  contactEmail: string;
}
