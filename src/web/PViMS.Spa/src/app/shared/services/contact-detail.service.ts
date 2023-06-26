import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { ContactDetailWrapperModel, ContactDetailModel } from '../models/contact-detail/contact-detail.detail.model';

@Injectable({ providedIn: 'root' })
export class ContactDetailService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getContactList(): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '50'});

      return this.Get<ContactDetailWrapperModel>('/contactdetails', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getContactDetail(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<ContactDetailModel>('/contactdetails', 'application/vnd.pvims.detail.v1+json', parameters);
  }     

  saveContact(id: number, model: any): any {
    return this.Put(`contactdetails/${id}`, model);
  }
}
