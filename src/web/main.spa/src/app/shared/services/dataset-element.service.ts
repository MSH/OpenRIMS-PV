import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { DatasetElementIdentifierWrapperModel } from '../models/work/dataset-element.identifier.model';
import { DatasetElementExpandedModel } from '../models/work/dataset-element.expanded.model';
import { DatasetElementSubWrapperModel } from '../models/work/dataset-element-sub.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class DatasetElementService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "";
  }

  getDatasetElements(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      parameters.push(<ParameterKeyValueModel> { key: 'elementName', value: filterModel.elementName});      

      return this.Get<DatasetElementIdentifierWrapperModel>('/datasetelements', 'application/vnd.main.identifier.v1+json', parameters);
  }

  getDatasetElementSubs(datasetElementId: number, filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    parameters.push(<ParameterKeyValueModel> { key: 'elementName', value: filterModel.elementName});      

    return this.Get<DatasetElementSubWrapperModel>(`/datasetelements/${datasetElementId}/elementsubs`, 'application/vnd.main.detail.v1+json', parameters);
}  

  getDatasetElementExpanded(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<DatasetElementExpandedModel>('/datasetelements', 'application/vnd.main.expanded.v1+json', parameters);
  }

  getDatasetElementSubDetail(datasetElementId: number, id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<DatasetElementExpandedModel>(`/datasetelements/${datasetElementId}/elementsubs`, 'application/vnd.main.detail.v1+json', parameters);
  }   

  saveDatasetElement(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`datasetelements`, model);
    }
    else {
      return this.Put(`datasetelements/${id}`, model);
    }
  }

  saveDatasetElementSub(datasetElementId: number, id: number, model: any): any {
    if(id == 0) {
      return this.Post(`datasetelements/${datasetElementId}/elementsubs`, model);
    }
    else {
      return this.Put(`datasetelements/${datasetElementId}/elementsubs/${id}`, model);
    }
  }    

  deleteDatasetElement(id: number): any {
    return this.Delete(`datasetelements/${id}`);
  }  

  deleteDatasetElementSub(datasetElementId: number, id: number): any {
    return this.Delete(`datasetelements/${datasetElementId}/elementsubs/${id}`);
  }    
}
