export interface ContactDetailWrapperModel {
    value:  ContactDetailModel[];
    recordCount: number;
}

export interface ContactDetailModel {
    id: number;
    contactType: string;
    contactFirstName: string;
    contactLastName: string;
    organisationType: string;
    organisationName: string;
    departmentName: string;
    streetAddress: string;
    city: string;
    state: string;
    postCode: string;
    countryCode: string;
    contactNumber: string;
    contactEmail: string;
}