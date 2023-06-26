export interface FormWrapperModel {
    value:  FormModel[];
    recordCount: number;    
}

export interface FormModel {
    id: number;
    created: string;
    formIdentifier: string;
    patientIdentifier: string;
    patientName: string;
    completeStatus: string;
    synchStatus: string;
    formType: string;
    hasAttachment: boolean;
    hasSecondAttachment: boolean;
}