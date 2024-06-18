import { AttributeValueForPostModel } from "../custom-attribute/attribute-value-for-post.model";

export interface PatientConditionForUpdateModel {
  id: number;
  index: number;
  condition: string;
  sourceDescription: string;
  sourceTerminologyMedDraId: number;
  onsetDate: any;
  outcomeDate?: any;
  outcome: string;
  treatmentOutcome: string;
  caseNumber: string;
  comments: string;
  attributes: AttributeValueForPostModel[];
}