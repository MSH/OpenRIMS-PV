import { AttributeValueModel } from "../attributevalue.model";
import { AppointmentDetailModel } from "../appointment/appointment.detail.model";
import { PatientStatusModel } from "./patient-status.model";
import { EncounterDetailModel } from "../encounter/encounter.detail.model";
import { PatientConditionDetailModel } from "./patient-condition.detail.model";
import { PatientClinicalEventDetailModel } from "./patient-clinical-event.detail.model";
import { PatientLabTestDetailModel } from "./patient-lab-test.detail.model";
import { PatientMedicationDetailModel } from "./patient-medication.detail.model";
import { AttachmentDetailModel } from "../attachment.detail.model";
import { CohortGroupPatientModel } from "./cohort-group-patient.model";
import { ConditionGroupPatientModel } from "./condition-group-patient.model";
import { ActivityExecutionStatusEventModel } from "../activity/activity-execution-status-event.model";

export interface PatientExpandedWrapperModel {
    value:  PatientExpandedModel[];
    recordCount: number;
}

export interface PatientExpandedModel {
    id: number;
    patientGuid: string;
    facilityName: string;
    organisationUnit: string;
    firstName: string;
    middleName: string;
    lastName: string;
    dateOfBirth: any;
    age: number;
    ageGroup: string;
    createdDetail: string;
    updatedDetail: string;
    latestEncounterDate: any;
    currentStatus: string;
    medicalRecordNumber: string;
    notes: string;

    patientAttributes: AttributeValueModel[];
    appointments: AppointmentDetailModel[];
    attachments: AttachmentDetailModel[];
    patientStatusHistories: PatientStatusModel[];
    encounters: EncounterDetailModel[];
    cohortGroups: CohortGroupPatientModel[];
    conditionGroups: ConditionGroupPatientModel[];
    activity: ActivityExecutionStatusEventModel[];

    patientClinicalEvents: PatientClinicalEventDetailModel[];
    patientConditions: PatientConditionDetailModel[];
    patientLabTests: PatientLabTestDetailModel[];
    patientMedications: PatientMedicationDetailModel[];
}