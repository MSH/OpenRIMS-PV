import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { EncounterDetailWrapperModel, EncounterDetailModel } from '../models/encounter/encounter.detail.model';
import { EncounterExpandedModel } from '../models/encounter/encounter.expanded.model';

@Injectable({ providedIn: 'root' })
export class EncounterService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "";
    }

    searchEncounter(filterModel: any): any {
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

        return this.Get<EncounterDetailWrapperModel>('/encounters', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getEncounterExpanded(patientId: number, id: number): any {
        let parameters: ParameterKeyValueModel[] = [];
        parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

        return this.Get<EncounterExpandedModel>(`/patients/${patientId}/encounters`, 'application/vnd.pvims.expanded.v1+json', parameters);
    }

    getEncounterDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<EncounterDetailModel>(`/patients/${patientId}/encounters`, 'application/vnd.pvims.detail.v1+json', parameters);
    } 

    saveEncounter(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post(`patients/${patientId}/encounters`, shallowModel);
      }
      else {
        return this.Put(`patients/${patientId}/encounters/${id}`, shallowModel);
      }
    }

    archiveEncounter(patientId: number, id: number, model: any): any {
      return this.Put(`patients/${patientId}/encounters/${id}/archive`, model);
    }
}
