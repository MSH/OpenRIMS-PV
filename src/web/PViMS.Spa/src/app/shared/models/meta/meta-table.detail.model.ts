import { LinkModel } from "../link.model";

export interface MetaTableDetailWrapperModel {
    value:  MetaTableDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface MetaTableDetailModel {
    id: number;
    metaTableGuid: string;
    tableName: string;
    friendlyName: string;
    friendlyDescription: string;
    tableType: string;
}