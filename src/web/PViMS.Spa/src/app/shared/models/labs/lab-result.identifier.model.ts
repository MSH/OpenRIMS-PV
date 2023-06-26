import { LinkModel } from "../link.model";

export interface LabResultIdentifierWrapperModel {
    value:  LabResultIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface LabResultIdentifierModel {
    id: number;
    labResultName: string;
    active: string;
}