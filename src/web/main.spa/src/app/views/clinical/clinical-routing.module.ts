import { Routes } from '@angular/router';

import { EncounterSearchComponent } from './encounter/encounter-search/encounter-search.component';
import { AppointmentSearchComponent } from './appointment/appointment-search/appointment-search.component';
import { EncounterViewComponent } from './encounter/encounter-view/encounter-view.component';
import { SynchroniseComponent } from './form/synchronise/synchronise.component';
import { PatientSearchComponent } from './patient/patient-search/patient-search.component';
import { PatientViewComponent } from './patient/patient-view/patient-view.component';
import { FeedbackSearchComponent } from './feedback/feedback-search/feedback-search.component';
import { CohortSearchComponent } from './cohort/cohort-search/cohort-search.component';
import { CohortEnrolmentListComponent } from './cohort/cohort-enrolment-list/cohort-enrolment-list.component';
import { PendingChangesGuard } from 'app/shared/guards/component-can-deactive';
import { FormListComponent } from './form/form-list/form-list.component';
import { FormSelectComponent } from './form/form-select/form-select.component';
import { FormDynamicComponent } from './form/form-dynamic/form-dynamic.component';

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
      path: 'form-select',
      component: FormSelectComponent,
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
      path: 'form-dynamic/:metaFormId/:formId',
      component: FormDynamicComponent,
      canDeactivate: [PendingChangesGuard],
      data: { title: 'Clinical Form Generator', breadcrumb: 'Clinical Form Generator' }
    }
  ]
  }
];
