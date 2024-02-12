import { LinkModel } from "../link.model";

export interface LabTestIdentifierWrapperModel {
    value:  LabTestIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface LabTestIdentifierModel {
    id: number;
    labTestName: string;
    active: string;
}