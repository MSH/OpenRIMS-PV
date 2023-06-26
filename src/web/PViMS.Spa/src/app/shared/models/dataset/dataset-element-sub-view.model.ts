import { SelectionDataItemModel } from "../custom-attribute/selection-data-item.model";

export interface DatasetElementSubViewModel {
  datasetElementSubId: number;
  datasetElementSubName: string;
  datasetElementSubType: string;
  datasetElementSubDisplayName: string;
  datasetElementSubHelp: string;
  datasetElementSubSystem: boolean;
  required: boolean;
  stringMaxLength?: number;
  numericMinValue?: number;
  numericMaxValue?: number;
  selectionDataItems: SelectionDataItemModel[];
}