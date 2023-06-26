import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { MetaPageDetailWrapperModel } from '../models/meta/meta-page.detail.model';
import { EMPTY } from 'rxjs';
import { MetaPageExpandedModel } from '../models/meta/meta-page.expanded.model';
import { WidgetListItemModel } from '../models/meta/widget-list-item.model';

@Injectable({ providedIn: 'root' })
export class MetaPageService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getAllMetaPages(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getMetaPages(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as MetaPageDetailWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<MetaPageDetailWrapperModel>(next.href, 'application/vnd.pvims.detail.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as MetaPageDetailWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getMetaPages(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<MetaPageDetailWrapperModel>('/metapages', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getMetaPage(id: number, header: 'detail' | 'expanded' ): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<MetaPageExpandedModel>('/metapages', `application/vnd.pvims.${header}.v1+json`, parameters);
  }

  getMetaWidget(metaPageId: number, id: number, header: 'detail' ): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<MetaPageExpandedModel>(`/metapages/${metaPageId}/widgets`, `application/vnd.pvims.${header}.v1+json`, parameters);
  }

  saveMetaPage(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`metapages`, model);
    }
    else {
      return this.Put(`metapages/${id}`, model);
    }
  }

  deleteMetaPage(id: number): any {
    return this.Delete(`metapages/${id}`);
  }

  saveMetaWidget(metaPageId: number, id: number, model: any, contentItems: WidgetListItemModel[]): any {
    if(id == 0) {
      return this.Post(`metapages/${metaPageId}/widgets`, model);
    }
    else {
      if(model.widgetType == 'ItemList') {
        model.listItems = contentItems.map(({ title, content }) => ({ title, content }));
      }
      if(model.widgetType == 'SubItems') {
        model.subItems = contentItems.map(({ title, subTitle, contentPage }) => ({ title, subTitle, contentPage }));
      }

      return this.Put(`metapages/${metaPageId}/widgets/${id}`, model);
    }
  }

  moveMetaWidget(metaPageId: number, id: number, model: any): any {
    return this.Put(`metapages/${metaPageId}/widgets/${id}/move`, model);
  }

  deleteMetaWidget(metaPageId: number, id: number): any {
    return this.Delete(`metapages/${metaPageId}/widgets/${id}`);
  }
}
