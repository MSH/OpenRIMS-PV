export interface AdverseEventReportWrapperModel {
    value:  AdverseEventReportModel[];
    recordCount: number;
}

export interface AdverseEventReportModel {
    adverseEvent: string;
    stratificationCriteria: string;
    patientCount: number;
}