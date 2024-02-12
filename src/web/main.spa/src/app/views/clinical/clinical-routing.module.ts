import { Routes } from '@angular/router';

import { EncounterSearchComponent } from './encounter/encounter-search/encounter-search.component';
import { AppointmentSearchComponent } from './appointment/appointment-search/appointment-search.component';
import { EncounterViewComponent } from './encounter/encounter-view/encounter-view.component';
import { FormAComponent } from './form/form-a/form-a.component';
import { FormBComponent } from './form/form-b/form-b.component';
import { FormCComponent } from './form/form-c/form-c.component';
import { SynchroniseComponent } from './form/synchronise/synchronise.component';
import { PatientSearchComponent } from './patient/patient-search/patient-search.component';
import { PatientViewComponent } from './patient/patient-view/patient-view.component';
import { FeedbackSearchComponent } from './feedback/feedback-search/feedback-search.component';
import { CohortSearchComponent } from './cohort/cohort-search/cohort-search.component';
import { CohortEnrolmentListComponent } from './cohort/cohort-enrolment-list/cohort-enrolment-list.component';
import { FormADRComponent } from './form/form-adr/form-adr.component';
import { LandingComponent } from './form/landing/landing.component';
import { PendingChangesGuard } from 'app/shared/guards/component-can-deactive';
import { FormListComponent } from './form/form-list/form-list.component';
import { FormADRDynamicComponent } from './form/form-adr-dynamic/form-adr-dynamic.component';

export const ClinicalRoutes: Routes = [
  {
    path: '',
    children: [{
      path: 'patientsearch',
      component: PatientSearchComponent,
      data: { title: 'Search For a Patient', breadcrumb: 'Patient Search' }
    },
    {
      path: 'encountersearch',
      component: EncounterSearchComponent,
      data: { title: 'Search For an Encounter', breadcrumb: 'Encounter Search' }
    },
    {
      path: 'cohortsearch',
      component: CohortSearchComponent,
      data: { title: 'Search For a Cohort', breadcrumb: 'Cohorts' }
    },
    {
      path: 'cohortenrolment/:id',
      component: CohortEnrolmentListComponent,
      data: { title: 'Cohort View', breadcrumb: 'Cohort View' }
    },
    {
      path: 'appointmentsearch',
      component: AppointmentSearchComponent,
      data: { title: 'Search For an Appointment', breadcrumb: 'Appointments' }
    },
    {
      path: 'feedbacksearch',
      component: FeedbackSearchComponent,
      data: { title: 'Search For PV Feedback', breadcrumb: 'Feedback' }
    },
    {
      path: 'patientview/:id',
      component: PatientViewComponent,
      data: { title: 'Patient View', breadcrumb: 'Patient View' }
    },
    {
      path: 'encounterview/:patientId/:id',
      component: EncounterViewComponent,
      data: { title: 'Encounter View', breadcrumb: 'Encounter View' }
    },
    {
      path: 'form-landing',
      component: LandingComponent,
      data: { title: 'List All Forms for Capture', breadcrumb: 'Forms' }
    },
    {
      path: 'form-list/:type',
      component: FormListComponent,
      data: { title: 'List All Forms for Capture', breadcrumb: 'Forms' }
    },
    {
      path: 'synchronise/:type',
      component: SynchroniseComponent,
      data: { title: 'Synchronise', breadcrumb: 'Synchronise' }
    },
    {
      path: 'forma/:id',
      component: FormAComponent,
      data: { title: 'Baseline Form A', breadcrumb: 'Baseline Form A' }
    },
    {
      path: 'formb/:id',
      component: FormBComponent,
      data: { title: 'Follow-Up Form B', breadcrumb: 'Follow-Up Form B' }
    },
    {
      path: 'formc/:id',
      component: FormCComponent,
      data: { title: 'Pregnancy Form C', breadcrumb: 'Pregnancy Form C' }
    },
    {
      path: 'formadr/:id',
      component: FormADRComponent,
      canDeactivate: [PendingChangesGuard],
      data: { title: 'Adverse Drug Reaction Form', breadcrumb: 'Adverse Drug Reaction Form' }
    }
  ]
  }
];
