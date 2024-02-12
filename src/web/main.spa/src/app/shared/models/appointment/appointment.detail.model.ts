export interface AppointmentDetailWrapperModel {
    value:  AppointmentDetailModel[];
    recordCount: number;
}

export interface AppointmentDetailModel {
    id: number;
    patientId: number;
    appointmentDate: any;
    reason: string;
    didNotArrive: boolean;
    cancelled: string;
    cancellationReason: string;
    createdDetail: string;
    updatedDetail: string;
}