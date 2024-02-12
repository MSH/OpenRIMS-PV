import { PatientDetailModel } from "../patient/patient.detail.model";
import { DatasetCategoryViewModel } from "../dataset/dataset-category-view.model";

export interface EncounterDetailWrapperModel {
    value:  EncounterDetailModel[];
    recordCount: number;
}

export interface EncounterDetailModel {
    id: number;
    patientId: number;
    encounterGuid: string;
    encounterDate: any;
    notes: string;
    encounterType: string;
    createdDetail: string;
    updatedDetail: string;
    patient: PatientDetailModel;

    datasetCategories: DatasetCategoryViewModel[];
}