export interface DatasetCategoryDetailWrapperModel {
    value:  DatasetCategoryDetailModel[];
    recordCount: number;
}

export interface DatasetCategoryDetailModel {
    id: number;
    datasetCategoryName: string;
    categoryOrder: number;
    system: string;
    acute: string;
    chronic: string;
    elementCount: number;
    friendlyName: string;
    help: string;
}