export interface CustomAttributeIdentifierWrapperModel {
    value:  CustomAttributeIdentifierModel[];
}

export interface CustomAttributeIdentifierModel {
    id: number;
    extendableTypeName: string;
    attributeKey: string;
}