export interface CausalityReportWrapperModel {
    value:  CausalityReportModel[];
    recordCount: number;
}

export interface CausalityReportModel {
    firstName: string;
    lastName: string;
    facilityName: string;
    adverseEvent: string;
    serious: string;
    onsetDate: any;
    whoCausality: string;
    naranjoCausality: string;
    medicationIdentifier: string;
}