export interface DatasetElementSubWrapperModel {
    value:  DatasetElementSubModel[];
    recordCount: number;
}

export interface DatasetElementSubModel {
    id: number;
    elementName: string;
    fieldTypeName: string;
    fieldOrder: number;
    oid: string;
    defaultValue: string;
    system: string;
    friendlyName: string;
    help: string;
    mandatory: string;
    maxLength?: number;
    regEx: string;
    decimals?: number;
    maxSize?: number;
    minSize?: number;
    calculation: string;
    anonymise: string;
}