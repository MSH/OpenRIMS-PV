import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { ConditionIdentifierWrapperModel } from '../models/condition/condition.identifier.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { ConditionDetailWrapperModel, ConditionDetailModel } from '../models/condition/condition.detail.model';
import { OutcomeIdentifierWrapperModel } from '../models/condition/outcome.identifier.model';
import { TreatmentOutcomeIdentifierWrapperModel } from '../models/condition/treatment-outcome.identifier.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class ConditionService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "/conditions";
  }

  getAllConditions(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getConditionsByDetail(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as ConditionDetailWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<ConditionDetailWrapperModel>(next.href, 'application/vnd.main.detail.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as ConditionDetailWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getConditions(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    if(filterModel.active != undefined) {
      parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
    }

    return this.Get<ConditionIdentifierWrapperModel>('', 'application/vnd.main.identifier.v1+json', parameters);
  }

  getConditionsByDetail(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    if(filterModel.active != undefined) {
      parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
    }

    return this.Get<ConditionDetailWrapperModel>('', 'application/vnd.main.detail.v1+json', parameters);
  }

  getAllOutcomes(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getOutcomes(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as OutcomeIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<OutcomeIdentifierWrapperModel>(next.href, 'application/vnd.main.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as OutcomeIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getOutcomes(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<OutcomeIdentifierWrapperModel>('/outcomes', 'application/vnd.main.identifier.v1+json', parameters);
  }

  getAllTreatmentOutcomes(): any {
    // Return all outcomes from the API
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getTreatmentOutcomes(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as TreatmentOutcomeIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<TreatmentOutcomeIdentifierWrapperModel>(next.href, 'application/vnd.main.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as TreatmentOutcomeIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getTreatmentOutcomes(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<TreatmentOutcomeIdentifierWrapperModel>('/treatmentoutcomes', 'application/vnd.main.identifier.v1+json', parameters);
  }

  getConditionDetail(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<ConditionDetailModel>('/conditions', 'application/vnd.main.detail.v1+json', parameters);
  }

  saveCondition(id: number, model: any, conditionLabTestIds: number[], conditionMeddraIds: number[], conditionMedications: number[]): any {
    model.conditionLabTests = conditionLabTestIds;
    model.conditionMedDras = conditionMeddraIds;
    model.conditionMedications = conditionMedications;

    if(id == 0) {
      return this.Post(``, model);
    }
    else {
      return this.Put(`${id}`, model);
    }
  }

  deleteCondition(id: number): any {
    return this.Delete(`${id}`);
  }    
}
