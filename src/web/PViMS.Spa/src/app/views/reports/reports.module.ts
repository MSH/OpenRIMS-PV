import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { OutstandingVisitComponent } from './system/outstanding-visit/outstanding-visit.component';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { SharedModule } from 'app/shared/shared.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { RouterModule } from '@angular/router';
import { ReportRoutes } from './reports-routing.module';
import { CausalityComponent } from './system/causality/causality.component';
import { AdverseEventComponent } from './system/adverse-event/adverse-event.component';
import { AdverseEventFrequencyComponent } from './system/adverse-event-frequency/adverse-event-frequency.component';
import { PatientTreatmentComponent } from './system/patient-treatment/patient-treatment.component';
import { PatientMedicationComponent } from './system/patient-medication/patient-medication.component';
import { PatientListPopupComponent } from './shared/patient-list/patient-list.popup.component';
import { ReportListComponent } from './report-list/report-list.component';
import { ReportConfigurePopupComponent } from './shared/report-configure-popup/report-configure.popup.component';
import { GenericDeletePopupComponent } from './shared/generic-delete-popup/generic-delete.popup.component';
import { AttributeConfigurePopupComponent } from './report-list/attribute-configure-popup/attribute-configure.popup.component';
import { FilterConfigurePopupComponent } from './report-list/filter-configure-popup/filter-configure.popup.component';
import { ReportViewerComponent } from './report-viewer/report-viewer.component';

@NgModule({
  declarations: [
    OutstandingVisitComponent,
    AdverseEventComponent,
    AdverseEventFrequencyComponent,
    PatientTreatmentComponent,
    PatientMedicationComponent,
    CausalityComponent,
    PatientListPopupComponent,
    ReportListComponent,
    ReportViewerComponent,
    ReportConfigurePopupComponent,
    GenericDeletePopupComponent,
    AttributeConfigurePopupComponent,
    FilterConfigurePopupComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedComponentsModule,
    SharedMaterialModule,
    SharedModule,
    PerfectScrollbarModule,
    RouterModule.forChild(ReportRoutes)
  ],
  entryComponents:
  [
    PatientListPopupComponent,
    ReportConfigurePopupComponent,
    GenericDeletePopupComponent,
    AttributeConfigurePopupComponent,
    FilterConfigurePopupComponent
  ]
})
export class ReportsModule { }
