import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { ConfigIdentifierWrapperModel, ConfigIdentifierModel } from '../models/config/config.identifier.model';

@Injectable({ providedIn: 'root' })
export class ConfigService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getConfigList(): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '50'});

      return this.Get<ConfigIdentifierWrapperModel>('/configs', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getConfigIdentifier(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<ConfigIdentifierModel>('/configs', 'application/vnd.pvims.identifier.v1+json', parameters);
  }     

  saveConfig(id: number, model: any): any {
    return this.Put(`configs/${id}`, model);
  }
}
