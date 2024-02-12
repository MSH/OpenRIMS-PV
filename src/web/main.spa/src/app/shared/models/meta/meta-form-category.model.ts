import { MetaFormCategoryAttributeModel } from "./meta-form-category-attribute.model";

export interface MetaFormCategoryModel {
    id: number;
    metaTableName: string;
    metaFormCategoryGuid: string;
    categoryName: string;
    help: string;
    icon: string;
    attributes: MetaFormCategoryAttributeModel[];
}