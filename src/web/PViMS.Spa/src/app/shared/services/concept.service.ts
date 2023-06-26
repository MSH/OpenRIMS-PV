import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { ProductDetailWrapperModel } from '../models/concepts/product.detail.model';
import { ConceptIdentifierWrapperModel } from '../models/concepts/concept.identifier.model';
import { ProductIdentifierWrapperModel } from '../models/concepts/product.identifier.model';
import { ConceptDetailWrapperModel } from '../models/concepts/concept.detail.model';
import { FilterModel } from '../models/grid.model';
import { EMPTY } from 'rxjs';
import { expand, map, reduce } from 'rxjs/operators';
import { MedicationFormIdentifierWrapperModel } from '../models/concepts/medication-form.identifier.model';

@Injectable({ providedIn: 'root' })
export class ConceptService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getAllProducts(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getProducts(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as ProductIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<ProductIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as ProductIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getProducts(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      if(filterModel.active != undefined) {
        parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
      }
  
      return this.Get<ProductIdentifierWrapperModel>('/products', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getAllConcepts(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getConcepts(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as ConceptIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<ConceptIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as ConceptIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getConcepts(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      if(filterModel.active != undefined) {
        parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
      }
  
      return this.Get<ConceptIdentifierWrapperModel>('/concepts', 'application/vnd.pvims.identifier.v1+json', parameters);
  }
    
  searchConcepts(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm || ''});
    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    if(filterModel.active != undefined) {
      parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
    }

    return this.Get<ConceptIdentifierWrapperModel>('/concepts', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  searchConceptsByDetail(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm || ''});
    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    if(filterModel.active != undefined) {
      parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
    }

    return this.Get<ConceptDetailWrapperModel>('/concepts', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  searchProducts(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm || ''});
    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    if(filterModel.active != undefined) {
      parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
    }

    return this.Get<ProductIdentifierWrapperModel>('/products', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  searchProductsByDetail(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm || ''});
    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    if(filterModel.active != undefined) {
      parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
    }

    return this.Get<ProductDetailWrapperModel>('/products', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getAllMedicationForms(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getMedicationForms(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as MedicationFormIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<MedicationFormIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as MedicationFormIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getMedicationForms(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<MedicationFormIdentifierWrapperModel>('/medicationforms', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  saveProduct(productId: number, model: any): any {
    if(productId == 0) {
      return this.Post(`products`, model);
    }
    else {
      return this.Put(`products/${productId}`, model);
    }
  }

  deleteProduct(id: number): any {
    return this.Delete(`products/${id}`);
  }

  saveConcept(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`concepts`, model);
    }
    else {
      return this.Put(`concepts/${id}`, model);
    }
  }

  deleteConcept(id: number): any {
    return this.Delete(`concepts/${id}`);
  }
}
