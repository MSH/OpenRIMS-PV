import { LinkModel } from "../link.model";

export interface ConceptIdentifierWrapperModel {
    value:  ConceptIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface ConceptIdentifierModel {
    id: number;
    conceptName: string;
    displayName: string;
    active: string;
}