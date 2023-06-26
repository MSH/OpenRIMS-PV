import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';

@Injectable({ providedIn: 'root' })
export class EnrolmentService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
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
