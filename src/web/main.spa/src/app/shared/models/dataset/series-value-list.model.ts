import { SeriesValueListItemModel } from "./series-value-list-item.model";

export interface SeriesValueListModel {
  name: string;
  series: SeriesValueListItemModel[];
}