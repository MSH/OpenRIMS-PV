import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { NotificationModel } from '../models/user/notification.model';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AccountService extends BaseService {

    connected$ = new BehaviorSubject<boolean>(true);
    forcedOffline$ = new BehaviorSubject<boolean>(false);

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "/accounts";
    }

    login(model: any) {
        return this.Post(`login`, model);
    }

    register(model: any) {
        return this.Post(`Register`, model);
    }

    forgotPassword(model: any) {
        return this.Post(`ForgotPassword`, model);
    }

    resetPassword(model: any) {
        return this.Post(`ResetPassword`, model);
    }

    getNotifications(): any {
      let parameters: ParameterKeyValueModel[] = [];

      return this.Get<NotificationModel[]>(`/accounts/notifications`, 'application/vnd.pvims.identifier.v1+json', parameters);
    }

    setConnectionStatus(connected: boolean): void {
      this.connected$.next(connected);
    }

    setForcedOffline(forced: boolean): void {
      this.forcedOffline$.next(forced);
      if(forced) {
        this.connected$.next(false);
      }
    }
}
