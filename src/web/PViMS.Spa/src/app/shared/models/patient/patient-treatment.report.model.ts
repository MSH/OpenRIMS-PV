import { PatientListModel } from "./patient-list.model";

export interface PatientTreatmentReportWrapperModel {
    value:  PatientTreatmentReportModel[];
    recordCount: number;
}

export interface PatientTreatmentReportModel {
    facilityId: number;
    facilityName: string;
    patientCount: number;
    patientWithNonSeriousEventCount: number;
    patientWithSeriousEventCount: number;
    eventPercentage: number;
    patients: PatientListModel[];
}