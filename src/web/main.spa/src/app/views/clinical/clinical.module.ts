import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RouterModule } from '@angular/router';
import { ClinicalRoutes } from './clinical-routing.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedModule } from 'app/shared/shared.module';
import { QuillModule } from 'ngx-quill';

import { EncounterSearchComponent } from './encounter/encounter-search/encounter-search.component';
import { AppointmentSearchComponent } from './appointment/appointment-search/appointment-search.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { EncounterViewComponent } from './encounter/encounter-view/encounter-view.component';
import { SynchroniseComponent } from './form/synchronise/synchronise.component';
import { WebcamModule } from 'ngx-webcam';
import { AttachmentCapturePopupComponent } from './form/form-list/attachment-capture-popup/attachment-capture.popup.component';
import { AttachmentViewPopupComponent } from './form/form-list/attachment-view-popup/attachment-view.popup.component';
import { PatientSearchComponent } from './patient/patient-search/patient-search.component';
import { PatientViewComponent } from './patient/patient-view/patient-view.component';
import { PatientUpdatePopupComponent } from './patient/patient-view/patient-update-popup/patient-update.popup.component';
import { PatientAddPopupComponent } from './patient/patient-search/patient-add-popup/patient-add.popup.component';
import { EncounterPopupComponent } from './patient/patient-view/encounter-popup/encounter.popup.component';
import { DnaPopupComponent } from './shared/dna-popup/dna.popup.component';
import { EncounterUpdatePopupComponent } from './encounter/encounter-view/encounter-update-popup/encounter-update.popup.component';
import { EncounterDeletePopupComponent } from './encounter/encounter-view/encounter-delete-popup/encounter-delete.popup.component';
import { ClinicalEventPopupComponent } from './shared/clinical-event-popup/clinical-event.popup.component';
import { ConditionPopupComponent } from './shared/condition-popup/condition.popup.component';
import { GenericDeletePopupComponent } from './shared/generic-delete-popup/generic-delete.popup.component';
import { ConditionViewPopupComponent } from './shared/condition-view-popup/condition-view.popup.component';
import { GenericArchivePopupComponent } from './shared/generic-archive-popup/generic-archive.popup.component';
import { FormDeletePopupComponent } from './form/form-list/form-delete-popup/form-delete.popup.component';
import { MedicationPopupComponent } from './shared/medication-popup/medication.popup.component';
import { LabTestPopupComponent } from './shared/lab-test-popup/lab-test.popup.component';
import { FeedbackSearchComponent } from './feedback/feedback-search/feedback-search.component';
import { CohortSearchComponent } from './cohort/cohort-search/cohort-search.component';
import { CohortPopupComponent } from './cohort/cohort-search/cohort-popup/cohort.popup.component';
import { CohortDeletePopupComponent } from './cohort/cohort-search/cohort-delete-popup/cohort-delete.popup.component';
import { CohortEnrolmentListComponent } from './cohort/cohort-enrolment-list/cohort-enrolment-list.component';
import { FormADRComponent } from './form/form-adr/form-adr.component';
import { AttachmentAddPopupComponent } from './shared/attachment-add-popup/attachment-add.popup.component';
import { ClinicalEventTaskPopupComponent } from './feedback/clinical-event-task-popup/clinical-event-task.popup.component';
import { FormADRMedicationPopupComponent } from './shared/form-adr-medication-popup/form-adr-medication.popup.component';
import { FormListComponent } from './form/form-list/form-list.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ClinicalEventViewPopupComponent } from './shared/clinical-event-view-popup/clinical-event-view.popup.component';
import { AppointmentPopupComponent } from './patient/patient-view/appointment-popup/appointment.popup.component';
import { FormGuidelinesPopupComponent } from './form/form-guidelines-popup/form-guidelines.popup.component';
import { FormSelectComponent } from './form/form-select/form-select.component';
import { FormDynamicComponent } from './form/form-dynamic/form-dynamic.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedComponentsModule,
    SharedMaterialModule,
    SharedModule,
    PerfectScrollbarModule,
    QuillModule,
    WebcamModule,
    NgApexchartsModule,
    RouterModule.forChild(ClinicalRoutes)
  ],
  declarations: 
  [
    AppointmentSearchComponent,
    AppointmentPopupComponent,
    AttachmentAddPopupComponent,
    AttachmentCapturePopupComponent,
    AttachmentViewPopupComponent,
    ClinicalEventPopupComponent,
    ClinicalEventTaskPopupComponent,
    ClinicalEventViewPopupComponent,
    CohortEnrolmentListComponent,
    CohortSearchComponent,
    CohortPopupComponent,
    CohortDeletePopupComponent,
    ConditionPopupComponent,
    ConditionViewPopupComponent,
    DnaPopupComponent,
    EncounterDeletePopupComponent,
    EncounterPopupComponent,
    EncounterSearchComponent,
    EncounterUpdatePopupComponent,
    EncounterViewComponent,
    FeedbackSearchComponent,
    FormADRComponent,
    FormADRMedicationPopupComponent,
    FormDeletePopupComponent,
    FormDynamicComponent,
    FormGuidelinesPopupComponent,
    FormListComponent,
    FormSelectComponent,
    GenericArchivePopupComponent,
    GenericDeletePopupComponent,
    LabTestPopupComponent,
    MedicationPopupComponent,
    PatientAddPopupComponent,
    PatientSearchComponent,
    PatientUpdatePopupComponent,
    PatientViewComponent,
    SynchroniseComponent
  ],
  entryComponents:
  [
    AppointmentPopupComponent,
    AttachmentAddPopupComponent,
    AttachmentCapturePopupComponent,
    AttachmentViewPopupComponent,
    ClinicalEventPopupComponent,
    ClinicalEventTaskPopupComponent,
    ClinicalEventViewPopupComponent,
    ConditionPopupComponent,
    ConditionViewPopupComponent,
    FormADRMedicationPopupComponent,
    FormDeletePopupComponent,
    FormGuidelinesPopupComponent,
    PatientAddPopupComponent,
    PatientUpdatePopupComponent,
    GenericDeletePopupComponent,
    GenericArchivePopupComponent,
    MedicationPopupComponent,
    LabTestPopupComponent,
    EncounterPopupComponent,
    EncounterUpdatePopupComponent,
    EncounterDeletePopupComponent,
    DnaPopupComponent,
    CohortPopupComponent,
    CohortDeletePopupComponent
  ]
})
export class ClinicalModule { }
