import { LinkModel } from "../link.model";
import { RiskFactorOptionModel } from "./risk-factor-option.model";

export interface RiskFactorDetailWrapperModel {
    value:  RiskFactorDetailModel[];
    recordCount: number;
    links: LinkModel[];
}

export interface RiskFactorDetailModel {
    id: number;
    factorName: string;
    active: string;
    criteria: string;
    display: string;
    system: boolean;
    options: RiskFactorOptionModel[];
}