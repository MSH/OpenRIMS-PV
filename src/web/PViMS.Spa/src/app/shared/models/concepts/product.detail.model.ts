export interface ProductDetailWrapperModel {
    value:  ProductDetailModel[];
    recordCount: number;
}

export interface ProductDetailModel {
    id: number;
    productName: string;
    displayName: string;
    active: string;
    conceptDisplayName: string;
    conceptName: string;
    strength: string;
    formName: string;
    manufacturer: string;
}