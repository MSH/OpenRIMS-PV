import { AttributeValueModel } from "../attributevalue.model";

export interface PatientLabTestDetailModel {
    id: number;
    patientLabTestGuid: string;
    labTest: string;
    testDate: any;
    testResultCoded: string;
    testResultValue: string;
    testUnit: string;
    referenceLower: string;
    referenceUpper: string;
    labTestAttributes: AttributeValueModel[];
}