import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RouterModule } from '@angular/router';
import { AnalyticalRoutes } from './analytical-routing.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedModule } from 'app/shared/shared.module';
import { SpontaneousAnalyserComponent } from './analyser/spontaneous-analyser/spontaneous-analyser.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ActivityHistoryComponent } from './report/activity-history/activity-history.component';
import { ActivityStatusChangePopupComponent } from './report/activity-status-change-popup/activity-status-change.popup.component';
import { ReportSearchComponent } from './report/report-search/report-search.component';
import { NaranjoPopupComponent } from './report/report-search/naranjo-popup/naranjo.popup.component';
import { SetMeddraPopupComponent } from './report/report-search/set-meddra-popup/set-meddra.popup.component';
import { DatasetInstancePopupComponent } from './report/report-search/dataset-instance-popup/dataset-instance.popup.component';
import { LandingComponent } from './landing/landing.component';
import { WhoPopupComponent } from './report/report-search/who-popup/who.popup.component';
import { ActiveAnalyserComponent } from './analyser/active-analyser/active-analyser.component';
import { ReportTaskListComponent } from './report/report-task/report-task-list.component';
import { ReportTaskAddPopupComponent } from './report/report-task/report-task-add-popup/report-task-add.popup.component';
import { ChangeTaskDetailsPopupComponent } from './report/report-task/change-task-details-popup/change-task-details.popup.component';
import { SetClassificationPopupComponent } from './report/report-search/set-classification/set-classification.popup.component';
import { MedicationListPopupComponent } from './report/report-search/medications-popup/medication-list.popup.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ClinicalDetailsComponent } from './report/clinical-details/clinical-details.component';

@NgModule({
  declarations: [
    ActiveAnalyserComponent,
    ActivityHistoryComponent,
    ActivityStatusChangePopupComponent,
    ChangeTaskDetailsPopupComponent,
    ClinicalDetailsComponent,
    DatasetInstancePopupComponent,
    LandingComponent,
    MedicationListPopupComponent,
    NaranjoPopupComponent,
    ReportSearchComponent, 
    ReportTaskAddPopupComponent,
    ReportTaskListComponent,
    SetClassificationPopupComponent,
    SetMeddraPopupComponent, 
    SpontaneousAnalyserComponent,
    WhoPopupComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    SharedComponentsModule,
    SharedMaterialModule,
    SharedModule,
    NgApexchartsModule,
    PerfectScrollbarModule,
    RouterModule.forChild(AnalyticalRoutes)
  ],
  entryComponents:
  [
    ActivityStatusChangePopupComponent,
    ChangeTaskDetailsPopupComponent,
    DatasetInstancePopupComponent,
    MedicationListPopupComponent,
    NaranjoPopupComponent,
    ReportTaskAddPopupComponent,    
    SetClassificationPopupComponent,
    SetMeddraPopupComponent, 
    WhoPopupComponent
  ]  
})
export class AnalyticalModule { }
