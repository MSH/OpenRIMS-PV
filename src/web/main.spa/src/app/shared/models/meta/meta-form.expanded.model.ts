import { MetaFormCategoryModel } from "./meta-form-category.model";

export interface MetaFormExpandedModel {
    id: number;
    formName: string;
    metaFormGuid: string;
    system: string;
    cohortGroupId: number;
    cohortGroup: string;
    actionName: string;
    version: string;
    categories: MetaFormCategoryModel[];
}