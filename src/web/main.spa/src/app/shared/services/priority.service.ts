import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { PriorityIdentifierWrapperModel } from '../models/encounter/priority.identifier.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class PriorityService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "";
  }

  getAllPriorities(): any {
    // Return all facilities from the API
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getPriorities(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as PriorityIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<PriorityIdentifierWrapperModel>(next.href, 'application/vnd.main.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as PriorityIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getPriorities(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<PriorityIdentifierWrapperModel>('/priorities', 'application/vnd.main.identifier.v1+json', parameters);
  }
}
