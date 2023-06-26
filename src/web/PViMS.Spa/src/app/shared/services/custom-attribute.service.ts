import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { CustomAttributeDetailModel, CustomAttributeDetailWrapperModel } from '../models/custom-attribute/custom-attribute.detail.model';
import { CustomAttributeIdentifierWrapperModel } from '../models/custom-attribute/custom-attribute.identifier.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CustomAttributeService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "/customattributes";
    }

    getPatientCustomAttributeSearchableList(): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'ExtendableTypeName', value: 'Patient'});
        parameters.push(<ParameterKeyValueModel> { key: 'IsSearchable', value: 'true'});
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '50'});
        
        return this.Get<CustomAttributeIdentifierWrapperModel>('', 'application/vnd.pvims.identifier.v1+json', parameters);
    }

    getAllCustomAttributes(extendableTypeName: string): any {
      // Return all facilities from the API
      let filter = new FilterModel();
      filter.recordsPerPage = 50;
      filter.currentPage = 1;

      return this.getCustomAttributes(extendableTypeName, filter)
        .pipe( 
          expand(response => {
            let typedResponse = response as CustomAttributeDetailWrapperModel;
            let next = typedResponse.links.find(l => l.rel == 'nextPage');
            return next ? this.GetByAddress<CustomAttributeDetailWrapperModel>(next.href, 'application/vnd.pvims.detail.v1+json') : EMPTY;
          }),
          map(response => {
            let typedResponse = response as CustomAttributeDetailWrapperModel;
            return typedResponse.value;
          }),
          reduce((accData, data) => accData.concat(data), [])
        );
    }

    getCustomAttributes(extendableTypeName: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'ExtendableTypeName', value: extendableTypeName});
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      
      return this.Get<CustomAttributeDetailWrapperModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getCustomAttributeDetail(id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<CustomAttributeDetailModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }    

    saveCustomAttribute(id: number, model: any): any {
      if(id == 0) {
        return this.Post('', model);
      }
      else {
        return this.Put(`${id}`, model);
      }
    }

    saveSelectionValue(customAttributeId: number, model: any): any {
      return this.Post(`${customAttributeId}/selection`, model);
    }

    deleteCustomAttribute(id: number): any {
      return this.Delete(`${id}`);
    }

    deleteSelectionValue(customAttributeId: number, key: string): any {
      return this.Delete(`${customAttributeId}/selection/${key}`);
    }
}
