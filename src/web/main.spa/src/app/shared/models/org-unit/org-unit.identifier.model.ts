import { LinkModel } from "../link.model";

export interface OrgUnitIdentifierWrapperModel {
    value:  OrgUnitIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface OrgUnitIdentifierModel {
    id: number;
    orgUnitName: string;
    orgUnitType: string;
}