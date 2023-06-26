import { LinkModel } from "../link.model";
import { AnalyserResultModel } from "./analyser-result.model";
import { SeriesValueListModel } from "../dataset/series-value-list.model";

export interface AnalyserTermDetailWrapperModel {
    value:  AnalyserTermDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface AnalyserTermDetailModel {
    terminologyMeddraId: number;
    meddraTerm: string;
    results: AnalyserResultModel[];
    relativeRiskSeries: SeriesValueListModel[];
    exposedCaseSeries: SeriesValueListModel[];
}