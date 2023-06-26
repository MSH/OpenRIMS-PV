export interface DatasetCategoryElementDetailWrapperModel {
    value:  DatasetCategoryElementDetailModel[];
    recordCount: number;
}

export interface DatasetCategoryElementDetailModel {
    id: number;
    elementName: string;
    datasetElementId: number;
    fieldOrder: number;
    system: string;
    acute: string;
    chronic: string;
    friendlyName: string;
    help: string;
}