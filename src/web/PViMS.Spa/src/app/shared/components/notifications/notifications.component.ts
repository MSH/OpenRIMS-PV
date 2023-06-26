import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { Subscription, interval } from 'rxjs';
import { NotificationModel } from 'app/shared/models/user/notification.model';
import { EventService } from 'app/shared/services/event.service';
import { _events } from 'app/config/events';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ["./notifications.component.scss"]
})
export class NotificationsComponent implements OnInit {
  isNotificationOpen: boolean = false;

  subscription: Subscription;
  pingInterval: number;
  checking: boolean;

  notifications: NotificationModel[] = [];
  readNotifications: NotificationModel[] = [];

  viewModel: ViewModel = new ViewModel();  

  constructor(
    private router: Router,
    public accountService: AccountService,
    protected eventService: EventService
  ) 
  {
    this.getNotifications();

    // set up interval for pinging API
    this.pingInterval = 900;
    const source = interval(this.pingInterval*1000);
    
    this.subscription = source.subscribe(val => 
      {
        if (this.viewModel.connected) {
          this.getNotifications();
        }        
      });     
  }

  ngOnInit() {
    const self = this;
    self.accountService.connected$.subscribe(val => {
      self.viewModel.connected = val;
    });
  }  

  getNotifications(): void {
    // Only execute if user is not logged out
    if(!this.accountService.hasToken())
    {
      return;
    }

    this.checking = true;
    this.eventService.broadcast(_events.check_notification_event);
    this.accountService.getNotifications()
      .subscribe(result => {
        console.log(result);
        this.notifications = result;
        this.checking = false;
      }, error => {
        this.checking = false;
      });
  }
  
  selectAction(route: string): void {
    this.isNotificationOpen = false;
    this.router.navigate([route]);
  }
}

class ViewModel {
  connected: boolean = true;
}
