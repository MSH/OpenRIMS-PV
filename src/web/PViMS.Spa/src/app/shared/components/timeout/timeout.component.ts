import { Component, OnInit } from "@angular/core";
import { AccountService } from "app/shared/services/account.service";
import { Subscription } from "rxjs";
import { EventService } from "app/shared/services/event.service";
import { _events } from "app/config/events";
import { UserIdleService } from "angular-user-idle";
import { Router } from "@angular/router";
import { _routes } from "app/config/routes";

@Component({
  selector: "app-timeout",
  templateUrl: "./timeout.component.html",
  styleUrls: ["./timeout.component.scss"]
})
export class TimeoutComponent implements OnInit {
  isTimerOpen: boolean = false;
  
  subscription: Subscription;
  pingInterval: number;
  checking: boolean;

  constructor(
    protected _router: Router,
    public accountService: AccountService,
    protected eventService: EventService,
    protected userIdle: UserIdleService
  ) {
    
    let self = this;

    //Start watching for user inactivity.
    console.log('start watching');
    self.userIdle.startWatching();
  
    self.userIdle.onTimerStart().subscribe(count => 
      self.handleOnTimer(count)
    );

    self.userIdle.onTimeout().subscribe(() =>
      this.logout()
    );
  }

  ngOnInit() {
  }

  handleOnTimer(count: number) {
    let self = this;
    self.isTimerOpen = true;
    console.log(count)
  }

  continue(): void {
    let self = this;
    self.userIdle.resetTimer();
    self.isTimerOpen = false;
  }

  logout(): void {
    let self = this;
    self.userIdle.resetTimer();
    self.accountService.removeToken();
    self._router.navigate([_routes.security.login])
  }
}