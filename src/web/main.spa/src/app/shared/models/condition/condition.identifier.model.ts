import { LinkModel } from "../link.model";

export interface ConditionIdentifierWrapperModel {
    value:  ConditionIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface ConditionIdentifierModel {
    id: number;
    conditionName: string;
    active: string;
}