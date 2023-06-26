import { LinkModel } from "../link.model";

export interface MetaFormDetailWrapperModel {
    value:  MetaFormDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface MetaFormDetailModel {
    id: number;
    system: string;
    version: string;
    formName: string;
    actionName: string;
    synchedCount: number;
    completedCount: number;
    unsynchedCount: number;
    metaFormGuid: string;
}