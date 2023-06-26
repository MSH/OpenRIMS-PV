import { AttributeValueForPostModel } from "../custom-attribute/attribute-value-for-post.model";

export interface PatientLabTestForUpdateModel {
  id: number;
  index: number;
  labTest: string;
  testDate: any;
  testResultCoded: string;
  testResultValue: string;
  testUnit: string;
  referenceLower: string;
  referenceUpper: string;
  attributes: AttributeValueForPostModel[];
}