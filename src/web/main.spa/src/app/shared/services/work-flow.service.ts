import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { WorkFlowDetailModel } from '../models/work-flow/work-flow.detail.model';
import { WorkFlowSummaryModel } from '../models/work-flow/work-flow.summary.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class WorkFlowService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "/workflow";
  }

  getWorkFlowDetail(id: string): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<WorkFlowDetailModel>('', 'application/vnd.main.detail.v1+json', parameters);
  }

  getWorkFlowSummary(id: string): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<WorkFlowSummaryModel>('', 'application/vnd.main.summary.v1+json', parameters);
  }

  downloadDataset(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    let url = `/workflow/${filterModel.workflowId}`;
    url += filterModel.datasetCohortGroupId != undefined ? `?cohortGroupId=${filterModel.datasetCohortGroupId}` : ``;
    
    return this.Download(url, 'application/vnd.main.dataset.v1+json', parameters);
  }
}