export interface DatasetElementIdentifierWrapperModel {
    value:  DatasetElementIdentifierModel[];
    recordCount: number;
}

export interface DatasetElementIdentifierModel {
    id: number;
    datasetElementGuid: string;
    elementName: string;
    fieldTypeName: string;
}