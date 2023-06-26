import { MetaOperatorModel } from "./meta-operator.model";

export interface MetaColumnExpandedWrapperModel {
    value:  MetaColumnExpandedModel[];
    recordCount: number;
}

export interface MetaColumnExpandedModel {
    id: number;
    metaColumnGuid: string;
    tableName: string;
    columnName: string;
    isIdentity: string;
    IsNullable: string;
    columnType: string;
    operators: MetaOperatorModel[];
}