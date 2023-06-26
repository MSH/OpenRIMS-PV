import { EnrolmentIdentifierModel } from "../cohort/enrolment.identifier.model";

export interface CohortGroupPatientModel {
    id: number;
    cohortName: string;
    cohortCode: string;
    startDate: any;
    conditionStartDate: any;
    condition: string;
    cohortGroupEnrolment: EnrolmentIdentifierModel;
}