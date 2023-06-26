import { LinkModel } from "../link.model";

export interface MedicationFormIdentifierWrapperModel {
    value:  MedicationFormIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface MedicationFormIdentifierModel {
    id: number;
    formName: string
}