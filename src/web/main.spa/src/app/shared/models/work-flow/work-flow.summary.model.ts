import { ClassificationSummaryModel } from "./classification-summary.model";

export interface WorkFlowSummaryWrapperModel {
    value:  WorkFlowSummaryModel[];
    recordCount: number;
}

export interface WorkFlowSummaryModel {
    id: number;
    workFlowGuid: string;
    workFlowName: string;
    submissionCount: number;
    deletionCount: number;
    reportDataConfirmedCount: number;
    terminologyAndCausalityConfirmedCount: number;
    classifications: ClassificationSummaryModel[];
}