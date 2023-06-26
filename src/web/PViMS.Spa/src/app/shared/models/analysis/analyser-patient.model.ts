export interface AnalyserPatientWrapperModel {
    value:  AnalyserPatientModel[];
    recordCount: number;
}

export interface AnalyserPatientModel {
    patientName: string;
    medication: string;
    startDate: string;
    finishDate: string;
    daysContributed: number;
    experiencedReaction: string;
    riskFactor: string;
    riskFactorOption: string;
    factorMet: string;
}