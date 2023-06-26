export interface CohortGroupDetailWrapperModel {
    value:  CohortGroupDetailModel[];
    recordCount: number;
}

export interface CohortGroupDetailModel {
    id: number;
    cohortName: string;
    cohortCode: string;
    startDate: any;
    finishDate: any;
    conditionName: string;
    enrolmentCount: number;
}