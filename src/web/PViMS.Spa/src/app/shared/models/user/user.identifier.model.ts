import { LinkModel } from "../link.model";

export interface UserIdentifierWrapperModel {
    value:  UserIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface UserIdentifierModel {
    id: number;
    userName: string
    firstName: string
    lastName: string
}