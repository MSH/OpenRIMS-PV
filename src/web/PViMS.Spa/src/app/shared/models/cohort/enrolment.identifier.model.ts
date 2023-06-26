export interface EnrolmentIdentifierWrapperModel {
  value:  EnrolmentIdentifierModel[];
  recordCount: number;
}

export interface EnrolmentIdentifierModel {
    id: number;
    patientId: number;
    cohortGroupId: number;
    enroledDate: any;
    deenroledDate: any;
}