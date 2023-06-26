import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';

import { BaseComponent } from 'app/shared/base/base.component';
import { Router } from '@angular/router';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';

@Component({
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent extends BaseComponent implements OnInit {

  constructor(
    protected _router: Router,
    protected _location: Location,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
  ) { 
    super(_router, _location, popupService, accountService, eventService);
  }

  ngOnInit() {
  }

}
