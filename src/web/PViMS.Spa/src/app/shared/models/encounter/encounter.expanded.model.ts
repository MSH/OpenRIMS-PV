import { PatientDetailModel } from "../patient/patient.detail.model";
import { PatientClinicalEventDetailModel } from "../patient/patient-clinical-event.detail.model";
import { PatientConditionDetailModel } from "../patient/patient-condition.detail.model";
import { PatientLabTestDetailModel } from "../patient/patient-lab-test.detail.model";
import { PatientMedicationDetailModel } from "../patient/patient-medication.detail.model";
import { DatasetCategoryViewModel } from "../dataset/dataset-category-view.model";
import { SeriesValueListModel } from "../dataset/series-value-list.model";

export interface EncounterExpandedWrapperModel {
    value:  EncounterExpandedModel[];
    recordCount: number;
}

export interface EncounterExpandedModel {
    id: number;
    encounterGuid: string;
    encounterDate: any;
    notes: string;
    encounterType: string;
    createdDetail: string;
    updatedDetail: string;
    patient: PatientDetailModel;

    datasetCategories: DatasetCategoryViewModel[];
    patientClinicalEvents: PatientClinicalEventDetailModel[];
    patientConditions: PatientConditionDetailModel[];
    patientLabTests: PatientLabTestDetailModel[];
    patientMedications: PatientMedicationDetailModel[];
    weightSeries: SeriesValueListModel[];
}