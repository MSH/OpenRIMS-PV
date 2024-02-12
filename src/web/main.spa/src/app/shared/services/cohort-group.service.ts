import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { CohortGroupDetailWrapperModel, CohortGroupDetailModel } from '../models/cohort/cohort-group.detail.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { CohortGroupIdentifierWrapperModel } from '../models/cohort/cohort-group.identifier.model';
import { EMPTY } from 'rxjs';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class CohortGroupService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
      super(httpClient, eventService, config);
      this.apiController = "/cohortgroups";
  }

  getAllCohortGroups(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getCohortGroups(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as CohortGroupIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<CohortGroupIdentifierWrapperModel>(next.href, 'application/vnd.main.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as CohortGroupIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getCohortGroups(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<CohortGroupIdentifierWrapperModel>('', 'application/vnd.main.identifier.v1+json', parameters);
  }

  getCohortGroupsByDetail(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<CohortGroupDetailWrapperModel>('', 'application/vnd.main.detail.v1+json', parameters);
  }

  getCohortGroupEnrolmentsByDetail(cohortGroupId: number, filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<CohortGroupDetailWrapperModel>(`/cohortgroups/${cohortGroupId}/cohortgroupenrolments`, 'application/vnd.main.detail.v1+json', parameters);
  }

  getCohortGroupDetail(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<CohortGroupDetailModel>('', 'application/vnd.main.detail.v1+json', parameters);
  }

  saveCohortGroup(id: number, model: any): any {
    let shallowModel = this.transformModelForDate(model);
    if(id == 0) {
      return this.Post(``, shallowModel);
    }
    else {
      return this.Put(`${id}`, shallowModel);
    }
  }  

  deleteCohortGroup(id: number): any {
    return this.Delete(`${id}`);
  }    
}
