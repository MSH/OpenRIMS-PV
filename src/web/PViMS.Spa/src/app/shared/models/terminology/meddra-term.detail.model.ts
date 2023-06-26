import { MeddraTermIdentifierModel } from "./meddra-term.identifier.model";

export interface MeddraTermDetailWrapperModel {
    value:  MeddraTermDetailModel[];
    recordCount: number;
}

export interface MeddraTermDetailModel {
    id: number;
    medDraTerm: string;
    medDraCode: string;
    parentMedDraTerm: string;
    medDraTermType: string;
    medDraVersion: string;
    children: MeddraTermIdentifierModel[];
}