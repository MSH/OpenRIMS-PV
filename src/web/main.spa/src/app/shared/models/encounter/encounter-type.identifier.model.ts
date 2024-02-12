import { LinkModel } from "../link.model";

export interface EncounterTypeIdentifierWrapperModel {
    value:  EncounterTypeIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface EncounterTypeIdentifierModel {
    id: number;
    encounterTypeName: string;
 }