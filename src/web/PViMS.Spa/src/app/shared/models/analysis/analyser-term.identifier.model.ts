import { LinkModel } from "../link.model";

export interface AnalyserTermIdentifierWrapperModel {
    value:  AnalyserTermIdentifierModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface AnalyserTermIdentifierModel {
    terminologyMeddraId: number;
    meddraTerm: string;
}