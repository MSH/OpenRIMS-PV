import { LinkModel } from "../link.model";

export interface MetaPageDetailWrapperModel {
    value:  MetaPageDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface MetaPageDetailModel {
    id: number;
    metaPageGuid: string;
    pageName: string;
    pageDefinition: string;
    metaDefinition: string;
    breadCrumb: string;
    system: string;
    visible: string;
}