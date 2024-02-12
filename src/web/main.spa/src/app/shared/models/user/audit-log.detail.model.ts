import { LinkModel } from "../link.model";

export interface AuditLogDetailWrapperModel {
  value:  AuditLogDetailModel[];
  recordCount: number;
  links: LinkModel[];
}

export interface AuditLogDetailModel {
    id: number;
    auditType: string;
    actionDate: string;
    details: string;
    userFullName: string;
    hasLog: boolean;
}