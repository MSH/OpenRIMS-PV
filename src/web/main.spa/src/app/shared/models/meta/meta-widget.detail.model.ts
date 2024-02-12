import { WidgetListItemModel } from "./widget-list-item.model";

export interface MetaWidgetDetailWrapperModel {
    value:  MetaWidgetDetailModel[];
    recordCount: number;
}

export interface MetaWidgetDetailModel {
    id: number;
    metaWidgetGuid: string;
    widgetName: string;
    widgetDefinition: string;
    content: string;
    widgetType: string;
    widgetLocation: string;
    widgetStatus: string;
    icon: string;
    contentItems: WidgetListItemModel[];
}