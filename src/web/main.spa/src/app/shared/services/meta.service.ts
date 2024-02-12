import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { MetaTableDetailWrapperModel } from '../models/meta/meta-table.detail.model';
import { MetaColumnDetailWrapperModel } from '../models/meta/meta-column.detail.model';
import { MetaDependencyDetailWrapperModel } from '../models/meta/meta-dependency.detail.model';
import { MetaSummaryModel } from '../models/meta/meta-summary.model';
import { FilterModel } from '../models/grid.model';
import { EMPTY } from 'rxjs';
import { expand, map, reduce } from 'rxjs/operators';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class MetaService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "";
  }

  getAllMetaTables(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getMetaTables(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as MetaTableDetailWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<MetaTableDetailWrapperModel>(next.href, 'application/vnd.main.detail.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as MetaTableDetailWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getMetaTables(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaTableDetailWrapperModel>('/metatables', 'application/vnd.main.detail.v1+json', parameters);
  }

  getAllMetaColumns(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getMetaColumns(filter);
  }

  getMetaColumns(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaColumnDetailWrapperModel>('/metacolumns', 'application/vnd.main.detail.v1+json', parameters);
  }

  getMetaDependencies(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaDependencyDetailWrapperModel>('/metadependencies', 'application/vnd.main.detail.v1+json', parameters);
  }

  getMetaSummary(): any {
    let parameters: ParameterKeyValueModel[] = [];

    return this.Get<MetaSummaryModel>('/meta', 'application/vnd.main.detail.v1+json', parameters);
  }

  refresh(): any {
    return this.Post('meta', null);
  }  
}
