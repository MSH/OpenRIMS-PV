export interface RoleIdentifierWrapperModel {
    value:  RoleIdentifierModel[];
    recordCount: number;    
}

export interface RoleIdentifierModel {
    id: string;
    name: string;
}