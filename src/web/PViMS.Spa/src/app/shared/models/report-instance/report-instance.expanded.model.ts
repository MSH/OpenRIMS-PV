import { TerminologyMedDraModel } from "../terminologymeddra.model";
import { ReportInstanceMedicationDetailModel } from "./report-instance-medication.detail.model";
import { LinkModel } from "../link.model";
import { DatasetInstanceModel } from "../dataset/dataset-instance-model";
import { TaskModel } from "./task.model";
import { ActivityExecutionStatusEventModel } from "./activity-execution-status-event.model";

export interface ReportInstanceExpandedWrapperModel {
    value:  ReportInstanceExpandedModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface ReportInstanceExpandedModel {
    id: number;
    reportInstanceGuid: string;
    contextGuid: string;
    identifier: string;
    sourceIdentifier: string;
    terminologyMedDra?: TerminologyMedDraModel;
    patientIdentifier: string;
    patientId: number;
    patientClinicalEventId: number;
    activityExecutionStatusEventId: number;
    e2BInstance?: DatasetInstanceModel;
    spontaneousInstance?: DatasetInstanceModel;
    attachmentId: number;
    taskCount: number;
    createdDetail: string;
    updatedDetail: string;
    qualifiedName: string;
    currentStatus: string;
    medications: ReportInstanceMedicationDetailModel[];
    events: ActivityExecutionStatusEventModel[];
    tasks: TaskModel[];
    links: LinkModel[];
}