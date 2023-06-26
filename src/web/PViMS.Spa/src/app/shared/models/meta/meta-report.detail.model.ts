import { LinkModel } from "../link.model";

export interface MetaReportDetailWrapperModel {
    value:  MetaReportDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface MetaReportDetailModel {
    id: number;
    metaReportGuid: string;
    reportName: string;
    reportDefinition: string;
    metaDefinition: string;
    breadCrumb: string;
    system: string;
    reportStatus: string;
    reportType: string;
    coreEntity: string;
}