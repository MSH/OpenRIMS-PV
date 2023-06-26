import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedModule } from '../../shared/shared.module';
import { PublicRoutes } from './public.routing';
import { SpontaneousComponent } from './spontaneous/spontaneous.component';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { SpontaneousTablePopupComponent } from './spontaneous/spontaneous-table/spontaneous-table.popup.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedMaterialModule,
    SharedComponentsModule,
    SharedModule,
    FlexLayoutModule,
    PerfectScrollbarModule,
    RouterModule.forChild(PublicRoutes)
  ],
  declarations: [
    SpontaneousComponent,
    SpontaneousTablePopupComponent
  ],
  entryComponents: [
    SpontaneousTablePopupComponent
  ]
})
export class PublicModule { }
