import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { FacilityTypeIdentifierWrapperModel } from '../models/facility/facility-type.identifier.model';
import { FacilityDetailWrapperModel, FacilityDetailModel } from '../models/facility/facility.detail.model';
import { EMPTY } from 'rxjs';
import { expand, map, reduce } from 'rxjs/operators';
import { FilterModel } from '../models/grid.model';

@Injectable({ providedIn: 'root' })
export class FacilityService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);    
        this.apiController = "";
    }

    getAllFacilities(): any {
      // Return all facilities from the API
      let filter = new FilterModel();
      filter.recordsPerPage = 50;
      filter.currentPage = 1;

      return this.getFacilities(filter)
        .pipe( 
          expand(response => {
            let typedResponse = response as FacilityDetailWrapperModel;
            let next = typedResponse.links.find(l => l.rel == 'nextPage');
            return next ? this.GetByAddress<FacilityDetailWrapperModel>(next.href, 'application/vnd.pvims.detail.v1+json') : EMPTY;
          }),
          map(response => {
            let typedResponse = response as FacilityDetailWrapperModel;
            return typedResponse.value;
          }),
          reduce((accData, data) => accData.concat(data), [])
        );
    }

    getFacilityTypeList(): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '999'});

      return this.Get<FacilityTypeIdentifierWrapperModel>('/facilitytypes', 'application/vnd.pvims.identifier.v1+json', parameters);
    }     

    getFacilities(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<FacilityDetailWrapperModel>('/facilities', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getFacilityDetail(id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<FacilityDetailModel>('/facilities', 'application/vnd.pvims.detail.v1+json', parameters);
    }     

    saveFacility(id: number, model: any): any {
      if(id == 0) {
        return this.Post(`facilities`, model);
      }
      else {
        return this.Put(`facilities/${id}`, model);
      }
    }

    deleteFacility(id: number): any {
      return this.Delete(`facilities/${id}`);
    }
}
