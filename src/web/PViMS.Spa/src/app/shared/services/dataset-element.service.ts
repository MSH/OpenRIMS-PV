import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { DatasetElementIdentifierWrapperModel } from '../models/work/dataset-element.identifier.model';
import { DatasetElementExpandedModel } from '../models/work/dataset-element.expanded.model';

@Injectable({ providedIn: 'root' })
export class DatasetElementService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getDatasetElements(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      parameters.push(<ParameterKeyValueModel> { key: 'elementName', value: filterModel.elementName});      

      return this.Get<DatasetElementIdentifierWrapperModel>('/datasetelements', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getDatasetElementExpanded(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<DatasetElementExpandedModel>('/datasetelements', 'application/vnd.pvims.expanded.v1+json', parameters);
  }

  saveDatasetElement(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`datasetelements`, model);
    }
    else {
      return this.Put(`datasetelements/${id}`, model);
    }
  }

  deleteDatasetElement(id: number): any {
    return this.Delete(`datasetelements/${id}`);
  }  
}
