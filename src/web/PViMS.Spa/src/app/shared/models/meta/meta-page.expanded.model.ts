import { MetaWidgetDetailModel } from "./meta-widget.detail.model";
import { MetaWidgetIdentifierModel } from "./meta-widget.identifier.model";

export interface MetaPageExpandedWrapperModel {
    value:  MetaPageExpandedModel[];
    recordCount: number;
}

export interface MetaPageExpandedModel {
    id: number;
    metaPageGuid: string;
    pageName: string;
    pageDefinition: string;
    metaDefinition: string;
    breadCrumb: string;
    system: string;
    visible: string;
    unpublishedWidgets: MetaWidgetIdentifierModel[];
    widgets: MetaWidgetDetailModel[];
}