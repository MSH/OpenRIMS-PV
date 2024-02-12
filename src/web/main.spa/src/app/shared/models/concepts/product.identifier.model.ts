import { LinkModel } from "../link.model";

export interface ProductIdentifierWrapperModel {
    value:  ProductIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface ProductIdentifierModel {
    id: number;
    productName: string;
    displayName: string;
    active: string;
}