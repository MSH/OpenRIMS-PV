import { LinkModel } from "../link.model";

export interface TreatmentOutcomeIdentifierWrapperModel {
    value:  TreatmentOutcomeIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface TreatmentOutcomeIdentifierModel {
    id: number;
    treatmentOutcomeName: string
}