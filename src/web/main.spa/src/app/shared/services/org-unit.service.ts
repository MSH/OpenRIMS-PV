import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { EMPTY } from 'rxjs';
import { expand, map, reduce } from 'rxjs/operators';
import { FilterModel } from '../models/grid.model';
import { OrgUnitIdentifierWrapperModel } from '../models/org-unit/org-unit.identifier.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class OrgUnitService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
        super(httpClient, eventService, config);    
        this.apiController = "";
    }

    getAllOrgUnits(): any {
      // Return all org units from the API
      let filter = new FilterModel();
      filter.recordsPerPage = 50;
      filter.currentPage = 1;

      return this.getOrgUnits(filter)
        .pipe( 
          expand(response => {
            let typedResponse = response as OrgUnitIdentifierWrapperModel;
            let next = typedResponse.links.find(l => l.rel == 'nextPage');
            return next ? this.GetByAddress<OrgUnitIdentifierWrapperModel>(next.href, 'application/vnd.main.identifier.v1+json') : EMPTY;
          }),
          map(response => {
            let typedResponse = response as OrgUnitIdentifierWrapperModel;
            return typedResponse.value;
          }),
          reduce((accData, data) => accData.concat(data), [])
        );
    }

    getOrgUnits(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<OrgUnitIdentifierWrapperModel>('/orgunits', 'application/vnd.main.identifier.v1+json', parameters);
    }
}
