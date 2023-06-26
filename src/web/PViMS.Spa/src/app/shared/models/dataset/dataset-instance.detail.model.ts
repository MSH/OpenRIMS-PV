import { DatasetCategoryViewModel } from "../dataset/dataset-category-view.model";

export interface DatasetInstanceDetailWrapperModel {
    value:  DatasetInstanceDetailModel[];
    recordCount: number;
}

export interface DatasetInstanceDetailModel {
    id: number;
    datasetInstanceGuid: string;
    createdDetail: string;
    updatedDetail: string;

    datasetCategories: DatasetCategoryViewModel[];
}