import { Routes } from '@angular/router';
import { OutstandingVisitComponent } from './system/outstanding-visit/outstanding-visit.component';
import { AdverseEventComponent } from './system/adverse-event/adverse-event.component';
import { PatientTreatmentComponent } from './system/patient-treatment/patient-treatment.component';
import { AdverseEventFrequencyComponent } from './system/adverse-event-frequency/adverse-event-frequency.component';
import { CausalityComponent } from './system/causality/causality.component';
import { PatientMedicationComponent } from './system/patient-medication/patient-medication.component';
import { ReportListComponent } from './report-list/report-list.component';
import { ReportViewerComponent } from './report-viewer/report-viewer.component';

export const ReportRoutes: Routes = [
  {
    path: '',
    children: [{
      path: 'system/outstandingvisit',
      component: OutstandingVisitComponent,
      data: { title: 'Outstanding Visit Report', breadcrumb: 'Outstanding Visit Report' }
    },
    {
      path: 'system/causality',
      component: CausalityComponent,
      data: { title: 'Causality Report', breadcrumb: 'Causality Report' }
    },
    {
      path: 'system/adverseevent',
      component: AdverseEventComponent,
      data: { title: 'Adverse Event Report', breadcrumb: 'Adverse Event Report' }
    },
    {
      path: 'system/patienttreatment',
      component: PatientTreatmentComponent,
      data: { title: 'Patient on Treatment Report', breadcrumb: 'Patient on Treatment Report' }
    },
    {
      path: 'system/patientsdrugreport',
      component: PatientMedicationComponent,
      data: { title: 'Patients by Drug Report', breadcrumb: 'Patients by Drug Report' }
    },
    {
      path: 'system/adverseeventfrequency',
      component: AdverseEventFrequencyComponent,
      data: { title: 'Adverse Event By Class Report', breadcrumb: 'Adverse Event By Class Report' },
    },
    {
      path: 'reportlist',
      component: ReportListComponent,
      data: { title: 'View Report List', breadcrumb: 'Report List' }
    },
    {
      path: 'reportviewer/:id',
      component: ReportViewerComponent,
      data: { title: 'View Custom Report', breadcrumb: 'View Custom Report' },
      runGuardsAndResolvers: 'always'      
    }
  ]
  }
];