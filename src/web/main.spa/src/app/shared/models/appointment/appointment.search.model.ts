export interface AppointmentSearchWrapperModel {
    value:  AppointmentSearchModel[];
    recordCount: number;
}

export interface AppointmentSearchModel {
    id: number;
    patientId: number;
    encounterId: number;
    appointmentDate: any;
    firstName: string;
    lastName: string;
    currentFacility: string;
    reason: string;
    appointmentStatus: string;
}