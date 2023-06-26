export interface AttachmentDetailWrapperModel {
    value:  AttachmentDetailModel[];
    recordCount: number;
}

export interface AttachmentDetailModel {
    id: number;
    fileName: string;
    description: string;
    attachmentTyoe: string;
    createdDetail: string;
    updatedDetail: string;
}