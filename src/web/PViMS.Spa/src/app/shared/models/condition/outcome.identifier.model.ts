import { LinkModel } from "../link.model";

export interface OutcomeIdentifierWrapperModel {
    value:  OutcomeIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface OutcomeIdentifierModel {
    id: number;
    outcomeName: string
}