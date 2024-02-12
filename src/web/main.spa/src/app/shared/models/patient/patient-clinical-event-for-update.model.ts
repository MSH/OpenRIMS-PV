import { AttributeValueForPostModel } from "../custom-attribute/attribute-value-for-post.model";

export interface PatientClinicalEventForUpdateModel {
  id: number;
  index: number;
  patientIdentifier: string;
  sourceDescription: string;
  sourceTerminologyMedDraId?: number;
  onsetDate: any;
  resolutionDate?: any;
  attributes: AttributeValueForPostModel[];
}