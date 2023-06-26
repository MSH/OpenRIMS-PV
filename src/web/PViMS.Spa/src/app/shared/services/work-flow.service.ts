import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { WorkFlowDetailModel } from '../models/work-flow/work-flow.detail.model';
import { WorkFlowSummaryModel } from '../models/work-flow/work-flow.summary.model';

@Injectable({ providedIn: 'root' })
export class WorkFlowService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "/workflow";
  }

  getWorkFlowDetail(id: string): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<WorkFlowDetailModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  getWorkFlowSummary(id: string): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<WorkFlowSummaryModel>('', 'application/vnd.pvims.summary.v1+json', parameters);
  }

  downloadDataset(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    let url = `/workflow/${filterModel.workflowId}`;
    url += filterModel.datasetCohortGroupId != undefined ? `?cohortGroupId=${filterModel.datasetCohortGroupId}` : ``;
    
    return this.Download(url, 'application/vnd.pvims.dataset.v1+json', parameters);
  }
}