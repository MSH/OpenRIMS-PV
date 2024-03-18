import { SelectionDataItemModel } from "../custom-attribute/selection-data-item.model";

export interface MetaFormCategoryAttributeModel {
    id: number;
    metaFormCategoryAttributeGuid: string;
    attributeId: number;
    attributeName: string;
    label: string;
    help: string;
    selected: boolean;
    formAttributeType: string;
    required: boolean;
    stringMaxLength?: number;
    numericMinValue?: number;
    numericMaxValue?: number;
    selectionDataItems: SelectionDataItemModel[];    
}