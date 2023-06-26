import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { HolidayIdentifierWrapperModel, HolidayIdentifierModel } from '../models/config/holiday.identifier';

@Injectable({ providedIn: 'root' })
export class HolidayService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getHolidays(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<HolidayIdentifierWrapperModel>('/holidays', 'application/vnd.pvims.identifier.v1+json', parameters);
}  

  getHolidayIdentifier(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<HolidayIdentifierModel>('/holidays', 'application/vnd.pvims.identifier.v1+json', parameters);
  }     

  saveHoliday(id: number, model: any): any {
    // Format date to prevent utc time issues
    model.holidayDate = model.holidayDate.format("YYYY-MM-DD");
    if(id == 0) {
      return this.Post(`holidays`, model);
    }
    else {
      return this.Put(`holidays/${id}`, model);
    }  
  }

  deleteHoliday(id: number): any {
    return this.Delete(`holidays/${id}`);
  }
}
