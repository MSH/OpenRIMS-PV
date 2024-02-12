import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { OutstandingVisitReportWrapperModel } from '../models/outstandingvisit.report.model';
import { AppointmentDetailModel } from '../models/appointment/appointment.detail.model';

import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { AppointmentSearchWrapperModel } from '../models/appointment/appointment.search.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

const moment =  _moment;

@Injectable({ providedIn: 'root' })
export class AppointmentService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
        super(httpClient, eventService, config);
        this.apiController = '';
    }

    searchAppointment(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'facilityName', value: filterModel.facilityName });
      parameters.push(<ParameterKeyValueModel> { key: 'criteriaId', value: filterModel.criteriaId == null ? 0 : filterModel.criteriaId });
      if (filterModel.patientId != null && filterModel.patientId != '') {
          parameters.push(<ParameterKeyValueModel> { key: 'patientId', value: filterModel.patientId });
      }
      if (filterModel.firstName != null) {
          parameters.push(<ParameterKeyValueModel> { key: 'firstName', value: filterModel.firstName });
      }
      if (filterModel.lastName != null) {
          parameters.push(<ParameterKeyValueModel> { key: 'lastName', value: filterModel.lastName });
      }
      if (filterModel.searchFrom != null && filterModel.searchFrom != '') {
          parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
      }
      if (filterModel.searchTo != null && filterModel.searchTo != '') {
          parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
      }
      if (filterModel.customAttributeId > 0) {
        parameters.push(<ParameterKeyValueModel> { key: 'customAttributeId', value: filterModel.customAttributeId });
        parameters.push(<ParameterKeyValueModel> { key: 'customAttributeValue', value: filterModel.customAttributeValue });
      }
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<AppointmentSearchWrapperModel>('/appointments', 'application/vnd.main.search.v1+json', parameters);
    }

    getAppointmentDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });
  
      return this.Get<AppointmentDetailModel>(`/patients/${patientId}/appointments`, 'application/vnd.main.detail.v1+json', parameters);
    }     

    saveAppointment(patientId: number, id: number, model: any): any {
      let localModel = Object.assign({}, model);
      if(moment.isDate(localModel.appointmentDate))
      {
        localModel.appointmentDate = localModel.appointmentDate.toString("YYYY-MM-DD");
      }

      if(id == 0) {
        return this.Post(`patients/${patientId}/appointments`, localModel);
      }
      else {
        return this.Put(`patients/${patientId}/appointments/${id}`, model);
      }
    }

    markAppointmentAsDNA(patientId: number, id: number): any {
      return this.Put(`patients/${patientId}/appointments/${id}/dna`, null);
    }

    archiveAppointment(patientId: number, id: number, model: any): any {
      return this.Put(`patients/${patientId}/appointments/${id}/archive`, model);
    }

    getOutstandingVisitReport(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'facilityId', value: filterModel.facilityId == null ? 0 : filterModel.facilityId });
      parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
      parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<OutstandingVisitReportWrapperModel>('/appointments', 'application/vnd.main.outstandingvisitreport.v1+json', parameters);
    }    
}
