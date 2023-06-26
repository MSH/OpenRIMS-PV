import { WidgetListItemModel } from "./widget-list-item.model";

export interface MetaWidgetIdentifierWrapperModel {
    value:  MetaWidgetIdentifierModel[];
    recordCount: number;
}

export interface MetaWidgetIdentifierModel {
    id: number;
    metaWidgetGuid: string;
    widgetName: string;
}