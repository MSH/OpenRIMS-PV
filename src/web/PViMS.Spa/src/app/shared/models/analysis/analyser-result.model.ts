import { IncidenceRateModel } from "./incidence-rate.model";

export interface AnalyserResultModel {
    medication: string;
    medicationId: number;
    exposedIncidenceRate: IncidenceRateModel;
    nonExposedIncidenceRate: IncidenceRateModel;
    unadjustedRelativeRisk: number;
    adjustedRelativeRisk: number;
    confidenceIntervalLow: number;
    confidenceIntervalHigh: number;
}