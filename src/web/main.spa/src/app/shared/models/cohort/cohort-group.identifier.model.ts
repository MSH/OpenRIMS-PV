import { LinkModel } from "../link.model";

export interface CohortGroupIdentifierWrapperModel {
    value:  CohortGroupIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface CohortGroupIdentifierModel {
    id: number;
    cohortName: string;
    cohortCode: string;
    startDate: any;
}