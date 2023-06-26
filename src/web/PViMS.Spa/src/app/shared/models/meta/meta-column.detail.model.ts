export interface MetaColumnDetailWrapperModel {
    value:  MetaColumnDetailModel[];
    recordCount: number;
}

export interface MetaColumnDetailModel {
    id: number;
    metaColumnGuid: string;
    tableName: string;
    columnName: string;
    isIdentity: string;
    IsNullable: string;
    columnType: string;
}