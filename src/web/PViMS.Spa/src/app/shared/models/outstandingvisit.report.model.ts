import { AttributeValueModel } from "./attributevalue.model";

export interface OutstandingVisitReportWrapperModel {
    value:  OutstandingVisitReportModel[];
    recordCount: number;
}

export interface OutstandingVisitReportModel {
    patientId: number;
    firstName: string;
    lastName: string;
    facility: string;
    appointmentDate: any;
}