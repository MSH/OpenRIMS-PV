import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { DatasetDetailWrapperModel } from '../models/work/dataset.detail.model';
import { DatasetCategoryDetailWrapperModel } from '../models/work/dataset-category.detail.model';
import { DatasetInstanceDetailWrapperModel } from '../models/dataset/dataset-instance.detail.model';
import { DatasetCategoryElementDetailWrapperModel } from '../models/work/dataset-category-element.detail.model';
import { DatasetCategoryIdentifierModel } from '../models/work/dataset-category.identifier.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class DatasetService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "";
  }

  getDatasets(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<DatasetDetailWrapperModel>('/datasets', 'application/vnd.main.detail.v1+json', parameters);
  }

  getDatasetCategories(datasetid: number, filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<DatasetCategoryDetailWrapperModel>(`/datasets/${datasetid}/categories`, 'application/vnd.main.detail.v1+json', parameters);
  }

  getDatasetCategoryElements(datasetid: number, datasetCategoryId: number, filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<DatasetCategoryElementDetailWrapperModel>(`/datasets/${datasetid}/categories/${datasetCategoryId}/elements`, 'application/vnd.main.detail.v1+json', parameters);
  }

  getDatasetInstanceDetail(datasetid: number, id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<DatasetInstanceDetailWrapperModel>(`/datasets/${datasetid}/instances`, 'application/vnd.main.detail.v1+json', parameters);
  }

  getDataset(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<DatasetCategoryIdentifierModel>(`/datasets`, 'application/vnd.main.identifier.v1+json', parameters);
  }

  getSpontaneousDataset(): any {
    let parameters: ParameterKeyValueModel[] = [];

    return this.Get<DatasetCategoryIdentifierModel>(`/datasets`, 'application/vnd.main.spontaneousdataset.v1+json', parameters);
  }
  
  getDatasetCategory(datasetid: number, id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<DatasetCategoryIdentifierModel>(`/datasets/${datasetid}/categories`, 'application/vnd.main.identifier.v1+json', parameters);
  }

  saveDataset(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`datasets`, model);
    }
    else {
      return this.Put(`datasets/${id}`, model);
    }
  }

  saveDatasetCategory(datasetid: number, id: number, model: any): any {
    if(id == 0) {
      return this.Post(`datasets/${datasetid}/categories`, model);
    }
    else {
      return this.Put(`datasets/${datasetid}/categories/${id}`, model);
    }
  }

  saveDatasetCategoryElement(datasetid: number, datasetCategoryId, id: number, model: any): any {
    if(id == 0) {
      return this.Post(`datasets/${datasetid}/categories/${datasetCategoryId}/elements`, model);
    }
    else {
      return this.Put(`datasets/${datasetid}/categories/${datasetCategoryId}/elements/${id}`, model);
    }
  }

  deleteDataset(id: number): any {
    return this.Delete(`datasets/${id}`);
  } 

  deleteDatasetCategory(datasetid: number, id: number): any {
    return this.Delete(`datasets/${datasetid}/categories/${id}`);
  } 

  deleteDatasetCategoryElement(datasetid: number, datasetCategoryId: number, id: number): any {
    return this.Delete(`datasets/${datasetid}/categories/${datasetCategoryId}/elements/${id}`);
  } 
  
  saveSpontaneousInstance(id: number, allModels: any[]): any {
    let shallowModels = [];
    for (let model of allModels) {
      let shallowModel = this.transformModelForDate(model.elements);
      shallowModels.push(shallowModel);
    }
    return this.Put(`datasets/${id}/instances`, shallowModels);
  }
}