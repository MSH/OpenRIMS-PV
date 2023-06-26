import { PatientListModel } from "./patient-list.model";

export interface PatientMedicationReportWrapperModel {
    value:  PatientMedicationReportModel[];
    recordCount: number;
}

export interface PatientMedicationReportModel {
    conceptId: number;
    conceptName: string;
    patientCount: number;
    patients: PatientListModel[];
}