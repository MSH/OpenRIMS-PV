import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { MetaAttributeModel } from '../models/meta/meta-attribute.model';
import { MetaFilterModel } from '../models/meta/meta-filter.model';
import { MetaReportExpandedModel } from '../models/meta/meta-report.expanded.model';
import { MetaReportDetailWrapperModel } from '../models/meta/meta-report.detail.model';
import { FilterModel } from '../models/grid.model';
import { EMPTY } from 'rxjs';
import { expand, map, reduce } from 'rxjs/operators';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class MetaReportService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "";
  }

  getAllMetaReports(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getMetaReportsByDetail(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as MetaReportDetailWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<MetaReportDetailWrapperModel>(next.href, 'application/vnd.main.detail.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as MetaReportDetailWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getMetaReportsByDetail(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaReportDetailWrapperModel>('/metareports', 'application/vnd.main.detail.v1+json', parameters);
  }

  getMetaReportByDetail(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<MetaReportExpandedModel>('/metareports', `application/vnd.main.detail.v1+json`, parameters);
  }

  getMetaReportByExpanded(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<MetaReportExpandedModel>('/metareports', `application/vnd.main.expanded.v1+json`, parameters);
  }

  saveMetaReport(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`metareports`, model);
    }
    else {
      return this.Put(`metareports/${id}`, model);
    }
  }

  deleteMetaReport(id: number): any {
    return this.Delete(`metareports/${id}`);
  }

  saveMetaReportAttributes(metaReportId: number, model: any, attributes: MetaAttributeModel[], filters: MetaFilterModel[]): any {
    model.attributes = attributes;
    model.filters = filters;
    console.log(model);
    return this.Put(`metareports/${metaReportId}/attributes`, model);
  }

}
