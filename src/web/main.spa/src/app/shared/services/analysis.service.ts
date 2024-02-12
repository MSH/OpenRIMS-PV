import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { AnalyserTermIdentifierWrapperModel } from '../models/analysis/analyser-term.identifier.model';
import { AnalyserTermDetailModel } from '../models/analysis/analyser-term.detail.model';
import { AnalyserPatientWrapperModel } from '../models/analysis/analyser-patient.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class AnalysisService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "";
  }

  getAllAnalysisTermSets(filterModel: any, riskFactors: any[]): any {
    return this.getAnalysisTermSet(filterModel, riskFactors)
      .pipe( 
        expand(response => {
          let typedResponse = response as AnalyserTermIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<AnalyserTermIdentifierWrapperModel>(next.href, 'application/vnd.main.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as AnalyserTermIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getAnalysisTermSet(filterModel: any, riskFactors: any[]): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'conditionId', value: filterModel.conditionId == null ? 1 : filterModel.conditionId });
      parameters.push(<ParameterKeyValueModel> { key: 'cohortGroupId', value: filterModel.cohortGroupId == null ? 0 : filterModel.cohortGroupId });
      parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
      parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '50'});

      if(riskFactors.length > 0) {
        riskFactors.forEach(element => {
          parameters.push(<ParameterKeyValueModel> { key: 'riskFactorOptionNames', value: element.optionName});
        })
      }

      return this.Get<AnalyserTermIdentifierWrapperModel>(`/workflow/892F3305-7819-4F18-8A87-11CBA3AEE219/analysisterms`, 'application/vnd.main.identifier.v1+json', parameters);
  }

  getAnalysisTermSetByDetail(filterModel: any, termId: number, riskFactors: any[]): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'conditionId', value: filterModel.conditionId == null ? 1 : filterModel.conditionId });
    parameters.push(<ParameterKeyValueModel> { key: 'cohortGroupId', value: filterModel.cohortGroupId == null ? 0 : filterModel.cohortGroupId });
    parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
    parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '50'});

    if(riskFactors.length > 0) {
      riskFactors.forEach(element => {
        parameters.push(<ParameterKeyValueModel> { key: 'riskFactorOptionNames', value: element.optionName});
      })
    }

    return this.Get<AnalyserTermDetailModel>(`/workflow/892F3305-7819-4F18-8A87-11CBA3AEE219/analysisterms/${termId}`, 'application/vnd.main.detail.v1+json', parameters);
  }

  getAnalysisPatientSet(filterModel: any, termId: number, riskFactors: any[]): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'conditionId', value: filterModel.conditionId == null ? 1 : filterModel.conditionId });
    parameters.push(<ParameterKeyValueModel> { key: 'cohortGroupId', value: filterModel.cohortGroupId == null ? 0 : filterModel.cohortGroupId });
    parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
    parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    if(riskFactors.length > 0) {
      riskFactors.forEach(element => {
        parameters.push(<ParameterKeyValueModel> { key: 'riskFactorOptionNames', value: element.optionName});
      })
    }

    return this.Get<AnalyserPatientWrapperModel>(`/workflow/892F3305-7819-4F18-8A87-11CBA3AEE219/analysisterms/${termId}/patients`, 'application/vnd.main.analyserpatientset.v1+json', parameters);
  }
}