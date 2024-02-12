export interface HolidayIdentifierWrapperModel {
    value:  HolidayIdentifierModel[];
}

export interface HolidayIdentifierModel {
    id: number;
    holidayDate: any;
    description: string;
}