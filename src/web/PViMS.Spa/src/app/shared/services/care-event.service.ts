import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { CareEventIdentifierWrapperModel } from '../models/work/care-event.identifier.model';

@Injectable({ providedIn: 'root' })
export class CareEventService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getCareEvents(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<CareEventIdentifierWrapperModel>('/careevents', 'application/vnd.pvims.identifier.v1+json', parameters);
  }    

  saveCareEvent(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`careevents`, model);
    }
    else {
      return this.Put(`careevents/${id}`, model);
    }
  }

  deleteCareEvent(id: number): any {
    return this.Delete(`careevents/${id}`);
  }
}
