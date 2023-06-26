import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { WorkPlanIdentifierWrapperModel } from '../models/work/work-plan.identifier.model';
import { WorkPlanDetailWrapperModel } from '../models/work/work-plan.detail.model';

@Injectable({ providedIn: 'root' })
export class WorkPlanService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getWorkPlanList(): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '999'});

      return this.Get<WorkPlanIdentifierWrapperModel>('/workplans', 'application/vnd.pvims.identifier.v1+json', parameters);
  } 
  
  getWorkPlans(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<WorkPlanDetailWrapperModel>('/workplans', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  // getMedicationDetail(id: number): any {
  //     let parameters: ParameterKeyValueModel[] = [];
  //     parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

  //     return this.Get<MedicationDetailModel>('/medications', 'application/vnd.pvims.detail.v1+json', parameters);
  // }     

  // saveMedication(id: number, model: any): any {
  //   if(id == 0) {
  //     return this.Post(`medications`, model);
  //   }
  //   else {
  //     return this.Put(`medications/${id}`, model);
  //   }
  // }
}
