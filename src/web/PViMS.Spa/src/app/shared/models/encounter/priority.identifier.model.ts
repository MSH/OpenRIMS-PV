import { LinkModel } from "../link.model";

export interface PriorityIdentifierWrapperModel {
    value:  PriorityIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface PriorityIdentifierModel {
    id: number;
    priorityName: string;
 }