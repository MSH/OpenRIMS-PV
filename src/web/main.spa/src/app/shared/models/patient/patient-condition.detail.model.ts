import { AttributeValueModel } from "../attributevalue.model";

export interface PatientConditionDetailModel {
    id: number;
    patientConditionGuid: string;
    sourceDescription: string;
    sourceTerminologyMedDraId: number;
    meddraTerm: string;
    startDate: any;
    outcomeDate: any;
    outcome: string;
    treatmentOutcome: string;
    caseNumber: string;
    conditionAttributes: AttributeValueModel[];
}