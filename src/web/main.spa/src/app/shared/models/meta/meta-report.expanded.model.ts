import { MetaAttributeModel } from "./meta-attribute.model";
import { MetaFilterModel } from "./meta-filter.model";
import { MetaTableExpandedModel } from "./meta-table.expanded.model";

export interface MetaReportExpandedWrapperModel {
    value:  MetaReportExpandedModel[];
    recordCount: number;
}

export interface MetaReportExpandedModel {
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
    coreMetaTable: MetaTableExpandedModel;
    attributes: MetaAttributeModel[];
    filters: MetaFilterModel[];
}