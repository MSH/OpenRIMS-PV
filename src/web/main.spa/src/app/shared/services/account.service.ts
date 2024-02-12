import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { NotificationModel } from '../models/user/notification.model';
import { BehaviorSubject } from 'rxjs';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class AccountService extends BaseService {

    connected$ = new BehaviorSubject<boolean>(true);
    forcedOffline$ = new BehaviorSubject<boolean>(false);

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
        super(httpClient, eventService, config);
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

      return this.Get<NotificationModel[]>(`/accounts/notifications`, 'application/vnd.main.identifier.v1+json', parameters);
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
