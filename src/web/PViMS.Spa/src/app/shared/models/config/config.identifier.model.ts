export interface ConfigIdentifierWrapperModel {
    value:  ConfigIdentifierModel[];
}

export interface ConfigIdentifierModel {
    id: number;
    configType: string;
    configValue: string;
}