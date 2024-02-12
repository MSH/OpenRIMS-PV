import { TerminologyMedDraModel } from "../terminologymeddra.model";
import { ReportInstanceMedicationDetailModel } from "./report-instance-medication.detail.model";
import { LinkModel } from "../link.model";
import { DatasetInstanceModel } from "../dataset/dataset-instance-model";

export interface ReportInstanceDetailWrapperModel {
    value:  ReportInstanceDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface ReportInstanceDetailModel {
    id: number;
    reportInstanceGuid: string;
    contextGuid: string;
    identifier: string;
    sourceIdentifier: string;
    terminologyMedDra?: TerminologyMedDraModel;
    patientIdentifier: string;
    reportClassification: string;
    patientId: number;
    patientClinicalEventId: number;
    activityExecutionStatusEventId: number;
    e2BInstance?: DatasetInstanceModel;
    spontaneousInstance?: DatasetInstanceModel;
    attachmentId: number;
    taskCount: number;
    activeTaskCount: number;
    createdDetail: string;
    updatedDetail: string;
    qualifiedName: string;
    currentStatus: string;
    medications: ReportInstanceMedicationDetailModel[];
    links: LinkModel[];
}