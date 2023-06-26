import { Routes } from '@angular/router';
import { SpontaneousAnalyserComponent } from './analyser/spontaneous-analyser/spontaneous-analyser.component';
import { ActivityHistoryComponent } from './report/activity-history/activity-history.component';
import { ReportSearchComponent } from './report/report-search/report-search.component';
import { LandingComponent } from './landing/landing.component';
import { ActiveAnalyserComponent } from './analyser/active-analyser/active-analyser.component';
import { ReportTaskListComponent } from './report/report-task/report-task-list.component';
import { ClinicalDetailsComponent } from './report/clinical-details/clinical-details.component';

export const AnalyticalRoutes: Routes = [
  {
    path: '',
    children: [
    {
        path: 'landing',
        component: LandingComponent,
        data: { title: 'Landing', breadcrumb: 'Landing' }
    },
    {
      path: 'activeanalyser',
      component: ActiveAnalyserComponent,
      data: { title: 'Active Reporting Analyser', breadcrumb: 'Analyser' }
    },
    {
      path: 'spontaneousanalyser',
      component: SpontaneousAnalyserComponent,
      data: { title: 'Spontaneous Reporting Analyser', breadcrumb: 'Analyser' }
    },
    {
      path: 'reportsearch/:wuid',
      component: ReportSearchComponent,
      data: { title: 'Search For a Report', breadcrumb: 'Reports' },
      runGuardsAndResolvers: 'always'
    },
    {
      path: 'reportsearch/:wuid/:qualifiedName',
      component: ReportSearchComponent,
      data: { title: 'Search For a Report', breadcrumb: 'Reports' },
      runGuardsAndResolvers: 'always'
    },
    {
      path: 'activityhistory/:wuid/:id',
      component: ActivityHistoryComponent,
      data: { title: 'Report Activities', breadcrumb: 'Activity' }
    },
    {
      path: 'clinicaldetails/:patientId/:clinicalEventId',
      component: ClinicalDetailsComponent,
      data: { title: 'Report Clinical Details', breadcrumb: 'Report Clinical Details' }
    },
    {
      path: 'reporttask/:wuid/:id',
      component: ReportTaskListComponent,
      data: { title: 'Report Tasks', breadcrumb: 'Tasks' }
    }]
  }
];
