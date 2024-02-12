export interface WorkPlanDetailWrapperModel {
    value:  WorkPlanDetailModel[];
    recordCount: number;
}

export interface WorkPlanDetailModel {
    id: number;
    workPlanName: string;
    datasetName: string;
}