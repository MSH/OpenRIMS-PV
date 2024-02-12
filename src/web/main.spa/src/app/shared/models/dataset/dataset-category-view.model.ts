import { DatasetElementViewModel } from "./dataset-element-view.model";

export interface DatasetCategoryViewModel {
  datasetCategoryId: number;
  datasetCategoryName: string;
  datasetCategoryDisplayName: string;
  datasetCategoryHelp: string;
  datasetCategoryDisplayed: boolean;
  datasetElements: DatasetElementViewModel[];
}