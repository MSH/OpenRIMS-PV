export interface ActivityExecutionStatusEventModel {
  id: number;
  patientClinicalEventId: number;
  adverseEvent: string;
  activity: string;
  executionEvent: string;
  executedBy: string; 
  executedDate: string;
  comments: string; 
  receiptDate: string;
  receiptCode: string;
  patientSummaryFileId: number;
  patientExtractFileId: number;
  e2bXmlFileId: number;
}