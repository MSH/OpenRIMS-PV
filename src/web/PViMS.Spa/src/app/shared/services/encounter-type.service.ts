import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { EncounterTypeDetailWrapperModel } from '../models/encounter/encounter-type.detail.model';
import { FilterModel } from '../models/grid.model';
import { EncounterTypeIdentifierWrapperModel } from '../models/encounter/encounter-type.identifier.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class EncounterTypeService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getAllEncounterTypes(): any {
    // Return all facilities from the API
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getEncounterTypes(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as EncounterTypeIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<EncounterTypeIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as EncounterTypeIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getEncounterTypes(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<EncounterTypeIdentifierWrapperModel>('/encountertypes', 'application/vnd.pvims.identifier.v1+json', parameters);
}

  getEncounterTypesByDetail(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<EncounterTypeDetailWrapperModel>('/encountertypes', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  saveEncounterType(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`encountertypes`, model);
    }
    else {
      return this.Put(`encountertypes/${id}`, model);
    }
  }

  deleteEncounterType(id: number): any {
    return this.Delete(`encountertypes/${id}`);
  }    
}
