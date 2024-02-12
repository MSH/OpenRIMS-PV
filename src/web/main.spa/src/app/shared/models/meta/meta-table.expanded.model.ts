import { MetaColumnExpandedModel } from "./meta-column.expanded.model";

export interface MetaTableExpandedWrapperModel {
    value:  MetaTableExpandedModel[];
    recordCount: number;
}

export interface MetaTableExpandedModel {
    id: number;
    metaTableGuid: string;
    tableName: string;
    friendlyName: string;
    friendlyDescription: string;
    tableType: string;
    columns: MetaColumnExpandedModel[];
}