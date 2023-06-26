export interface FormValueWrapperModel {
    value:  FormValueModel[];
    recordCount: number;    
}

export interface FormValueModel {
    id?: number;
    formid: number;
    formUniqueIdentifier: string;
    formControlKey: string;
    formControlValue: string;
}