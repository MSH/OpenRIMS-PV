export interface AdverseEventFrequencyReportWrapperModel {
    value:  AdverseEventFrequencyReportModel[];
    recordCount: number;
}

export interface AdverseEventFrequencyReportModel {
    periodDisplay: string;
    systemOrganClass: string;
    grade1Count: number;
    grade2Count: number;
    grade3Count: number;
    grade4Count: number;
    grade5Count: number;
    gradeUnknownCount: number;
}