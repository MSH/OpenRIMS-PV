export interface CareEventIdentifierWrapperModel {
    value:  CareEventIdentifierModel[];
    recordCount: number;
}

export interface CareEventIdentifierModel {
    id: number;
    careEventName: string
}