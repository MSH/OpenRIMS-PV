import { LinkModel } from "../link.model";

export interface MeddraTermIdentifierWrapperModel {
    value:  MeddraTermIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface MeddraTermIdentifierModel {
    id: number;
    medDraTerm: string;
    medDraCode: string;
}