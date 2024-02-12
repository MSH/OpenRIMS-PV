import { LinkModel } from "../link.model";

export interface LabTestUnitIdentifierWrapperModel {
    value:  LabTestUnitIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface LabTestUnitIdentifierModel {
    id: number;
    labTestUnitName: string
}