import { DatasetElementSubViewModel } from "./dataset-element-sub-view.model";
import { SelectionDataItemModel } from "../custom-attribute/selection-data-item.model";

export interface DatasetElementViewModel {
  datasetElementId: number;
  datasetElementName: string;
  datasetElementDisplayName: string;
  datasetElementHelp: string;
  datasetElementType: string;
  datasetElementValue?: string;
  datasetElementDisplayed: boolean;
  datasetElementChronic: boolean;
  datasetElementSystem: boolean;
  datasetElementSubs: DatasetElementSubViewModel[];
  required: boolean;
  stringMaxLength?: number;
  numericMinValue?: number;
  numericMaxValue?: number;
  selectionDataItems: SelectionDataItemModel[];
}