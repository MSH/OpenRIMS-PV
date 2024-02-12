import { AttributeValueForPostModel } from "../custom-attribute/attribute-value-for-post.model";

export interface PatientForCreationModel {
  firstName: string;
  lastName: string;
  middleName: string;
  dateOfBirth: any;
  facilityName: string;
  conditionGroupId: number;
  meddraTermId: number;
  cohortGroupId?: number;
  enroledDate?: any;
  startDate: any;
  outcomeDate?: any;
  caseNumber: string;
  comments: string;
  encounterTypeId: number;
  priorityId: number;
  encounterDate: any;
  attributes: AttributeValueForPostModel[];
}