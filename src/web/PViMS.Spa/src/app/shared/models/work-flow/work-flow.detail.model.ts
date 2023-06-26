import { ActivitySummaryModel } from "./activity-summary.model";

export interface WorkFlowDetailWrapperModel {
    value:  WorkFlowDetailModel[];
    recordCount: number;
}

export interface WorkFlowDetailModel {
    id: number;
    workFlowGuid: string;
    workFlowName: string;
    newReportInstanceCount: number;
    analysisActivity: ActivitySummaryModel[];
    feedbackActivity: ActivitySummaryModel[];
}