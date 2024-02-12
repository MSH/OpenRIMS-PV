export interface DatasetDetailWrapperModel {
    value:  DatasetDetailModel[];
    recordCount: number;
}

export interface DatasetDetailModel {
    id: number;
    datasetName: string;
    active: string;
    system: string;
    help: string;
    contextType: string;
}