export interface EnrolmentDetailWrapperModel {
  value:  EnrolmentDetailModel[];
  recordCount: number;
}

export interface EnrolmentDetailModel {
  id: number;
  patientId: number;
  cohortGroupId: number;
  enroledDate: any;
  deenroledDate: any;
  fullName: string;
  facilityName: string;
  dateOfBirth: any;
  age: number;
  latestEncounterDate: any;
  currentWeight?: number;
  nonSeriousEventCount: number;
  seriousEventCount: number;
}