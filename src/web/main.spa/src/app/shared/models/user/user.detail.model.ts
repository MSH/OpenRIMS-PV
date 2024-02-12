import { UserFacilityModel } from "./user-facility.model";

export interface UserDetailModel {
    id: number;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    allowDatasetDownload: string;
    active: string;
    roles: string[];
    facilities: UserFacilityModel[];
}