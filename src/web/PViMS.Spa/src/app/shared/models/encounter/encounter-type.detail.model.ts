export interface EncounterTypeDetailWrapperModel {
    value:  EncounterTypeDetailModel[];
    recordCount: number;
}

export interface EncounterTypeDetailModel {
    id: number;
    encounterTypeName: string;
    help: string;
    workPlanName: string;
}