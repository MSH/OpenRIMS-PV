import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PageViewerComponent } from './page-viewer/page-viewer.component';
import { PageListComponent } from './page-list/page-list.component';
import { RouterModule } from '@angular/router';
import { InformationRoutes } from './information-routing.module';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { SharedModule } from 'app/shared/shared.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PageViewerPopupComponent } from './page-viewer/page-viewer-popup/page-viewer.popup.component';
import { PageConfigurePopupComponent } from './shared/page-configure-popup/page-configure.popup.component';
import { GenericDeletePopupComponent } from './shared/generic-delete-popup/generic-delete.popup.component';
import { WidgetConfigurePopupComponent } from './page-viewer/widget-configure-popup/widget-configure.popup.component';
import { WidgetMovePopupComponent } from './page-viewer/widget-move-popup/widget-move.popup.component';
import { QuillModule } from 'ngx-quill';
import { GeneralWidgetComponent } from './page-viewer/general-widget/general-widget.component';
import { SubitemWidgetComponent } from './page-viewer/subitem-widget/subitem-widget.component';
import { ItemListWidgetComponent } from './page-viewer/item-list-widget/item-list-widget.component';

@NgModule({
  declarations: [
    GenericDeletePopupComponent,
    PageViewerComponent, 
    PageListComponent,
    PageViewerPopupComponent,
    PageConfigurePopupComponent,
    WidgetConfigurePopupComponent,
    WidgetMovePopupComponent,
    GeneralWidgetComponent,
    SubitemWidgetComponent,
    ItemListWidgetComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedComponentsModule,
    SharedMaterialModule,
    SharedModule,
    PerfectScrollbarModule,
    QuillModule,
    RouterModule.forChild(InformationRoutes)
  ],
  entryComponents:
  [
    GenericDeletePopupComponent,
    PageViewerPopupComponent,
    PageConfigurePopupComponent,
    WidgetConfigurePopupComponent,
    WidgetMovePopupComponent
  ]
})
export class InformationModule { }
