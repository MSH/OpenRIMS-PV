export interface DatasetCategoryIdentifierWrapperModel {
    value:  DatasetCategoryIdentifierModel[];
    recordCount: number;
}

export interface DatasetCategoryIdentifierModel {
    id: number;
    datasetCategoryName: string;
}