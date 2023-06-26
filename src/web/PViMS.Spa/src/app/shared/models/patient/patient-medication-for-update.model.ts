import { AttributeValueForPostModel } from "../custom-attribute/attribute-value-for-post.model";

export interface PatientMedicationForUpdateModel {
  id: number;
  index: number;
  medication: string;
  sourceDescription: string;
  conceptId: number;
  productId?: number;
  startDate: any;
  endDate: any;
  dose: string;
  doseFrequency: string;
  doseUnit: string;
  attributes: AttributeValueForPostModel[];
}