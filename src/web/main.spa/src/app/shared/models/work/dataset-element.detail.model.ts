export interface DatasetElementDetailWrapperModel {
    value:  DatasetElementDetailModel[];
    recordCount: number;
}

export interface DatasetElementDetailModel {
    id: number;
    datasetElementGuid: string;
    elementName: string;
    oid: string;
    defaultValue: string;
    system: string;
    mandatory: string;
    maxLength?: number;
    regEx: string;
    decimals?: number;
    maxSize?: number;
    minSize?: number;
    calculation: string;
    anonymise: string;
    fieldTypeName: string;
}