import { Inject, Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { CausalityReportWrapperModel } from '../models/causality.report.model';
import { ActivityChangeModel } from '../models/activity/activity-change.model';
import { ReportInstanceDetailWrapperModel } from '../models/report-instance/report-instance.detail.model';
import { ReportInstanceExpandedModel } from '../models/report-instance/report-instance.expanded.model';
import { TaskModel } from '../models/report-instance/task.model';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Injectable({ providedIn: 'root' })
export class ReportInstanceService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService, @Inject(APP_CONFIG) config: AppConfig) {
        super(httpClient, eventService, config);
        this.apiController = "";
    }

    searchReportInstanceByActivity(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'qualifiedName', value: filterModel.qualifiedName });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      if(filterModel.activeReportsOnly != undefined) {
        parameters.push(<ParameterKeyValueModel> { key: 'activeReportsOnly', value: filterModel.activeReportsOnly});
      }

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.detail.v1+json', parameters);
    }

    searchReportInstanceByDate(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
      parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.detail.v1+json', parameters);
    }

    searchReportInstanceByTerm(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.detail.v1+json', parameters);
    }

    getNewReportInstancesByDetail(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.new.v1+json', parameters);
    }

    getAnalysisReportInstancesByDetail(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'qualifiedName', value: filterModel.qualifiedName });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.analysis.v1+json', parameters);
    }

    getFeedbackReportInstancesByDetail(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'qualifiedName', value: filterModel.qualifiedName });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.feedback.v1+json', parameters);
    }

    searchFeedbackInstanceByTerm(workFlowGuid: string, filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.feedback.v1+json', parameters);
    }

    getReportInstanceDetail(workFlowGuid: string, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<ReportInstanceDetailWrapperModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.detail.v1+json', parameters);
    } 

    getReportInstanceExpanded(workFlowGuid: string, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<ReportInstanceExpandedModel>(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.expanded.v1+json', parameters);
    } 

    getReportInstanceTaskDetail(workFlowGuid: string, reportInstanceId: number, reportInstanceTaskId: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: reportInstanceTaskId.toString() });

      return this.Get<TaskModel>(`/workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks`, 'application/vnd.main.detail.v1+json', parameters);
    } 

    getActivityChangeStatus(workFlowGuid: string, reportInstanceId: number, id: number, activityExecutionStatusId: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      let header = '';
      if(activityExecutionStatusId == 85)
      {
        header = 'application/vnd.main.activitystatusconfirm.v1+json';
      }
      return this.Get<ActivityChangeModel>(`/workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/activity`, header, parameters);
    }

    getCausalityReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'causalityCriteria', value: filterModel.criteriaId == null ? 1 : filterModel.criteriaId });
        parameters.push(<ParameterKeyValueModel> { key: 'facilityId', value: filterModel.facilityId == null ? 0 : filterModel.facilityId });
        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

        return this.Get<CausalityReportWrapperModel>(`/workflow/892F3305-7819-4F18-8A87-11CBA3AEE219/reportinstances`, 'application/vnd.main.causalityreport.v1+json', parameters);
    }

    downloadAttachment(workFlowGuid: string, reportInstanceId: number, activityExecutionStatusEventId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });
  
      return this.Download(`/workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/activity/${activityExecutionStatusEventId}/attachments`, 'application/vnd.main.attachment.v1+xml', parameters);
    }

    downloadSummary(workFlowGuid: string, reportInstanceId: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: reportInstanceId.toString() });
  
      return this.Download(`/workflow/${workFlowGuid}/reportinstances`, 'application/vnd.main.patientsummary.v1+json', parameters);
    }

    addTaskToReportInstanceCommand(workFlowGuid: string, reportInstanceId: number, model: any): any {
      return this.Post(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks`, model);
    }

    addCommentToReportInstanceTaskCommand(workFlowGuid: string, reportInstanceId: number, reportInstanceTaskId: number, model: any): any {
      return this.Post(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks/${reportInstanceTaskId}/comments`, model);
    }

    changeTaskDetailsCommand(workFlowGuid: string, reportInstanceId: number, reportInstanceTaskId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks/${reportInstanceTaskId}/details`, model);
    }

    changeTaskStatusCommand(workFlowGuid: string, reportInstanceId: number, reportInstanceTaskId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/tasks/${reportInstanceTaskId}/status`, model);
    }

    updateReportInstanceActivity(workFlowGuid: string, reportInstanceId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/activity`, model);
    }

    updateTerminology(workFlowGuid: string, reportInstanceId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/terminology`, model);
    }

    updateReportInstanceMedicationCausality(workFlowGuid: string, reportInstanceId: number, medicationId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/medications/${medicationId}/causality`, model);
    }

    createE2B(workFlowGuid: string, reportInstanceId: number): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/createe2b`, null);
    }

    updateReportInstanceClassification(workFlowGuid: string, reportInstanceId: number, model: any): any {
      return this.Put(`workflow/${workFlowGuid}/reportinstances/${reportInstanceId}/classification`, model);
    }
}
