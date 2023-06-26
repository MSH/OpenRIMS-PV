import { AttributeValueModel } from "../attributevalue.model";

export interface PatientMedicationDetailModel {
    id: number;
    patientMedicationGuid: string;
    sourceDescription: string;
    conceptId: number;
    productId: number;
    medication: string;
    dose: string;
    doseUnit: string;
    doseFrequency: string;
    startDate: any;
    endDate: any;
    indicationType: string;
    reasonForStopping: string;
    clinicianAction: string;
    challengeEffect: string;
    medicationAttributes: AttributeValueModel[];
}