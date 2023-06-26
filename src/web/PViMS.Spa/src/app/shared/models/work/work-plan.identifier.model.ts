export interface WorkPlanIdentifierWrapperModel {
    value:  WorkPlanIdentifierModel[];
    recordCount: number;
}

export interface WorkPlanIdentifierModel {
    id: number;
    workPlanName: string;
}