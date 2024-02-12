import { MetaFormCategoryModel } from "./meta-form-category.model";

export interface MetaFormExpandedModel {
    id: number;
    formName: string;
    metaFormGuid: string;
    system: string;
    cohortGroup: string;
    actionName: string;
    version: string;
    categories: MetaFormCategoryModel[];
}