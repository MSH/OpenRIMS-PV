import { Component, OnInit, Inject, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { EventService } from 'app/shared/services/event.service';
import { _events } from 'app/config/events';

@Component({
  templateUrl: './online-status.popup.component.html',
  styles: [`
    .error-status { color: red; }
    .connected-status { color: green; }
    .checking-status { color: black; }  
  `]  
})
export class OnlineStatusPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: OnlineStatusPopupData,
    public dialogRef: MatDialogRef<OnlineStatusPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit() {
    const self = this;
    self.accountService.connected$.subscribe(val => {
      console.log(`Marked connected in status check as ${val}`);
      self.viewModel.connected = val;
    });
    self.accountService.forcedOffline$.subscribe(val => {
      console.log(`Marked forced offline in status check as ${val}`);
      self.viewModel.forcedOffline = val;
    });    
  }

  ngAfterViewInit(): void {
    let self = this;
  }
  
  pingAPI(): void {
    const self = this;
    self.viewModel.checking = true;
    self.viewModel.pingStatusMessage = "Pinging ...."
    self.eventService.broadcast(_events.ping_event);    
    self.accountService.ping()
      .subscribe(result => {
        self.accountService.setConnectionStatus(true);
        self.viewModel.checking = false;
        self.viewModel.pingStatusMessage = "Ping successful ..."
      }, error => {
        self.accountService.setConnectionStatus(false);
        self.viewModel.checking = false;
        self.viewModel.pingStatusMessage = "Ping unsuccessful ..."
      });
  }
  
  goOffline(): void {
    const self = this;
    self.viewModel.forcedStatusMessage = "Going offline ...."
    self.accountService.setForcedOffline(true);
    self.viewModel.forcedStatusMessage = "In offline mode"
    self.viewModel.pingStatusMessage = "In offline mode"
  }
  
  goOnline(): void {
    const self = this;
    self.viewModel.forcedStatusMessage = "Going online ...."
    self.accountService.setForcedOffline(false);
    self.pingAPI();
  }  
}

export interface OnlineStatusPopupData {
  workFlowGuid: string;
  reportInstanceId: number;
  reportInstanceTaskId: number;
  title: string;
}

class ViewModel {
  checking: boolean = false;
  connected: boolean = true;
  forcedOffline: boolean = false;
  pingStatusMessage: string = '';
  forcedStatusMessage: string = '';
}