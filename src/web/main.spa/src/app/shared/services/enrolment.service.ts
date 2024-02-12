import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class EnrolmentService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
        super(httpClient, eventService, config);
        this.apiController = "";
    }

    saveEnrolment(patientId: number, cohortGroupId: number, model: any): any {
        model.cohortGroupId = cohortGroupId;
        return this.Post(`patients/${patientId}/cohortgroupenrolments`, model);
    }

    saveDeenrolment(patientId: number, cohortGroupEnrolmentId: number, model: any): any {
        return this.Put(`patients/${patientId}/cohortgroupenrolments/${cohortGroupEnrolmentId}`, model);
    }    

    archiveEnrolment(patientId: number, cohortGroupEnrolmentId: number, model: any): any {
      return this.Put(`patients/${patientId}/cohortgroupenrolments/${cohortGroupEnrolmentId}/archive`, model);
    }

}
