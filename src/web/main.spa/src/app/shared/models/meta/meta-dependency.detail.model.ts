export interface MetaDependencyDetailWrapperModel {
    value:  MetaDependencyDetailModel[];
    recordCount: number;
}

export interface MetaDependencyDetailModel {
    id: number;
    metaDependencyGuid: string;
    parentTableName: string;
    parentColumnName: string;
    referenceTableName: string;
    referenceColumnName: string;
}