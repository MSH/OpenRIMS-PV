import { AttributeValueModel } from "../attributevalue.model";
import { ReportInstanceMedicationDetailModel } from "../report-instance/report-instance-medication.detail.model";
import { ActivityExecutionStatusEventModel } from "../activity/activity-execution-status-event.model";
import { TaskModel } from "../report-instance/task.model";

export interface PatientClinicalEventExpandedModel {
    id: number;
    patientClinicalEventGuid: string;
    sourceDescription: string;
    sourceTerminologyMedDraId?: number;
    medDraTerm: string;
    onsetDate: any;
    reportDate: any;
    resolutionDate: any;
    isSerious: string;
    clinicalEventAttributes: AttributeValueModel[];
    reportInstanceId: number;
    setMedDraTerm: string;
    setClassification: string;
    medications: ReportInstanceMedicationDetailModel[];
    activity: ActivityExecutionStatusEventModel[];
    tasks: TaskModel[];
}