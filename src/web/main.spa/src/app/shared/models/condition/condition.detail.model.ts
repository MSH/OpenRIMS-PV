import { LinkModel } from "../link.model";
import { ConditionLabTestModel } from "./condition-lab-test.model";
import { ConditionMeddraModel } from "./condition-meddra.model";
import { CohortGroupIdentifierModel } from "../cohort/cohort-group.identifier.model";

export interface ConditionDetailWrapperModel {
    value:  ConditionDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface ConditionDetailModel {
    id: number;
    conditionName: string;
    chronic: string;
    active: string;
    conditionLabTests: ConditionLabTestModel[];
    conditionMedDras: ConditionMeddraModel[]; 
    cohortGroups: CohortGroupIdentifierModel[];
}