export interface WorkFlowIdentifierWrapperModel {
    value:  WorkFlowIdentifierModel[];
    recordCount: number;
}

export interface WorkFlowIdentifierModel {
    id: number;
    workFlowGuid: string;
    workFlowName: string;
}