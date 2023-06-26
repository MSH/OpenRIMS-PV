import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { RiskFactorDetailWrapperModel } from '../models/risk-factor/risk-factor.detail.model';

@Injectable({ providedIn: 'root' })
export class RiskFactorService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "/riskfactors";
    }

    getAllRiskFactors(): any {
      // Return all facilities from the API
      let filter = new FilterModel();
      filter.recordsPerPage = 50;
      filter.currentPage = 1;

      return this.getRiskFactors(filter)
        .pipe( 
          expand(response => {
            let typedResponse = response as RiskFactorDetailWrapperModel;
            let next = typedResponse.links.find(l => l.rel == 'nextPage');
            return next ? this.GetByAddress<RiskFactorDetailWrapperModel>(next.href, 'application/vnd.pvims.detail.v1+json') : EMPTY;
          }),
          map(response => {
            let typedResponse = response as RiskFactorDetailWrapperModel;
            return typedResponse.value;
          }),
          reduce((accData, data) => accData.concat(data), [])
        );
    }

    getRiskFactors(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      
      return this.Get<RiskFactorDetailWrapperModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }    
}
