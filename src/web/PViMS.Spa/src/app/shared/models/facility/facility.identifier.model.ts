import { LinkModel } from "../link.model";

export interface FacilityIdentifierWrapperModel {
    value:  FacilityIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface FacilityIdentifierModel {
    id: number;
    facilityName: string
}