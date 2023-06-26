import { SelectionDataItemModel } from "./selection-data-item.model";
import { LinkModel } from "../link.model";

export interface CustomAttributeDetailWrapperModel {
    value:  CustomAttributeDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface CustomAttributeDetailModel {
    id: number;
    extendableTypeName: string;
    attributeKey: string;
    attributeDetail: string;
    category: string;
    customAttributeType: string;
    required: boolean;
    isSearchable: boolean;
    stringMaxLength?: number;
    numericMinValue?: number;
    numericMaxValue?: number;
    selectionDataItems: SelectionDataItemModel[];
}