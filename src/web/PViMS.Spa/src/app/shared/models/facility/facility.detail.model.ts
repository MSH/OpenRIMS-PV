import { LinkModel } from "../link.model";

export interface FacilityDetailWrapperModel {
    value:  FacilityDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface FacilityDetailModel {
    id: number;
    facilityName: string;
    facilityType: string;
    facilityCode: string;
    contactNumber: string;
    mobileNumber: string;
    faxNumber: string;
    orgUnitId?: number;
}