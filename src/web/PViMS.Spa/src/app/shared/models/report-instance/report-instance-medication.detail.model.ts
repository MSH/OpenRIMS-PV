export interface ReportInstanceMedicationDetailModel {
    id: number;
    reportInstanceId: number;
    reportInstanceMedicationGuid: string;
    medicationIdentifier: string;
    naranjoCausality: string;
    whoCausality: string;
    startDate: any;
    endDate: any;
}