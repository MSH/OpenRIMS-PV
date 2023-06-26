export interface ActivityExecutionStatusEventModel {
  adverseEvent?: string;
  patientClinicalEventId?: number;
  activity: string;
  executionEvent: string;
  executedDate: string;
  comments: string;
}