import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportRoutes } from './administration-routing.module';
import { FormsModule } from '@angular/forms';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedMaterialModule } from 'app/shared/shared-material.module';
import { SharedModule } from 'app/shared/shared.module';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { RouterModule } from '@angular/router';
import { ConfigListComponent } from './system/config-list/config-list.component';
import { FacilityListComponent } from './system/facility-list/facility-list.component';
import { MedicineListComponent } from './reference/medicine-list/medicine-list.component';
import { UserListComponent } from './user/user-list/user-list.component';
import { LabTestListComponent } from './reference/lab-test-list/lab-test-list.component';
import { LabResultListComponent } from './reference/lab-result-list/lab-result-list.component';
import { ContactDetailListComponent } from './system/contact-detail-list/contact-detail-list.component';
import { HolidayListComponent } from './system/holiday-list/holiday-list.component';
import { ReportMetaViewComponent } from './system/report-meta-view/report-meta-view.component';
import { RoleListComponent } from './user/role-list/role-list.component';
import { CareEventListComponent } from './work/care-event-list/care-event-list.component';
import { DatasetElementListComponent } from './work/dataset-element-list/dataset-element-list.component';
import { DatasetListComponent } from './work/dataset-list/dataset-list.component';
import { EncounterTypeListComponent } from './work/encounter-type-list/encounter-type-list.component';
import { WorkPlanListComponent } from './work/work-plan-list/work-plan-list.component';
import { ConditionListComponent } from './reference/condition-list/condition-list.component';
import { AuditLogListComponent } from './audit-log-list/audit-log-list.component';
import { LandingComponent } from './landing/landing.component';
import { MeddraListComponent } from './reference/meddra-list/meddra-list.component';
import { ImportMeddraPopupComponent } from './reference/meddra-list/import-meddra-popup/import-meddra.popup.component';
import { FormListComponent } from './work/form-list/form-list.component';
import { FormPopupComponent } from './work/form-list/form-popup/form.popup.component';
import { FormDeletePopupComponent } from './work/form-list/form-delete-popup/form-delete.popup.component';
import { ConditionDeletePopupComponent } from './reference/condition-list/condition-delete-popup/condition-delete.popup.component';
import { ConditionPopupComponent } from './reference/condition-list/condition-popup/condition.popup.component';
import { LabTestSelectPopupComponent } from './shared/lab-test-select-popup/lab-test-select.popup.component';
import { DatasetCategoryListComponent } from './work/dataset-list/dataset-category-list/dataset-category-list.component';
import { DatasetCategoryElementListComponent } from './work/dataset-list/dataset-category-element-list/dataset-category-element-list.component';
import { DatasetCategoryElementDeletePopupComponent } from './work/dataset-list/dataset-category-element-list/dataset-category-element-delete-popup/dataset-category-element-delete.popup.component';
import { DatasetCategoryDeletePopupComponent } from './work/dataset-list/dataset-category-list/dataset-category-delete-popup/dataset-category-delete.popup.component';
import { DatasetElementSelectPopupComponent } from './shared/dataset-element-select-popup/dataset-element-select.popup.component';
import { DatasetCategoryElementPopupComponent } from './work/dataset-list/dataset-category-element-list/dataset-category-element-popup/dataset-category-element.popup.component';
import { ConceptListComponent } from './reference/concept-list/concept-list.component';
import { MedicationPopupComponent } from './reference/medicine-list/medication-popup/medication.popup.component';
import { MedicationDeletePopupComponent } from './reference/medicine-list/medication-delete-popup/medication-delete.popup.component';
import { ConceptPopupComponent } from './reference/concept-list/concept-popup/concept.popup.component';
import { GenericDeletePopupComponent } from './shared/generic-delete-popup/generic-delete.popup.component';
import { CareEventPopupComponent } from './work/care-event-list/care-event-popup/care-event.popup.component';
import { HolidayDeletePopupComponent } from './system/holiday-list/holiday-delete-popup/holiday-delete.popup.component';
import { HolidayPopupComponent } from './system/holiday-list/holiday-popup/holiday.popup.component';
import { FacilityPopupComponent } from './system/facility-list/facility-popup/facility.popup.component';
import { CareEventDeletePopupComponent } from './work/care-event-list/care-event-delete-popup/care-event-delete.popup.component';
import { FacilityDeletePopupComponent } from './system/facility-list/facility-delete-popup/facility-delete.popup.component';
import { DatasetElementDeletePopupComponent } from './work/dataset-element-list/dataset-element-delete-popup/dataset-element-delete.popup.component';
import { UserRolePopupComponent } from './user/user-list/user-role-popup/user-role.popup.component';
import { PasswordResetPopupComponent } from './user/user-list/password-reset-popup/password-reset.popup.component';
import { UserAddPopupComponent } from './user/user-list/user-add-popup/user-add.popup.component';
import { UserDeletePopupComponent } from './user/user-list/user-delete-popup/user-delete.popup.component';
import { UserUpdatePopupComponent } from './user/user-list/user-update-popup/user-update.popup.component';
import { UserFacilityPopupComponent } from './user/user-list/user-facility-popup/user-facility.popup.component';
import { CustomAttributeListComponent } from './work/custom-attribute-list/custom-attribute-list.component';
import { CustomAttributeAddPopupComponent } from './work/custom-attribute-list/custom-attribute-add-popup/custom-attribute-add.popup.component';
import { CustomAttributeDeletePopupComponent } from './work/custom-attribute-list/custom-attribute-delete-popup/custom-attribute-delete.popup.component';
import { CustomAttributeEditPopupComponent } from './work/custom-attribute-list/custom-attribute-edit-popup/custom-attribute-edit.popup.component';
import { SelectionItemPopupComponent } from './work/custom-attribute-list/selection-item-popup/selection-item.popup.component';
import { DatasetElementSubListComponent } from './work/dataset-element-sub-list/dataset-element-sub-list.component';
import { DatasetElementSubDeletePopupComponent } from './work/dataset-element-sub-list/dataset-element-sub-delete-popup/dataset-element-sub-delete.popup.component';
import { DatasetElementSubPopupComponent } from './work/dataset-element-sub-list/dataset-element-sub-popup/dataset-element-sub.popup.component';
import { FormViewerComponent } from './work/form-list/form-viewer/form-viewer.component';
import { CategoryConfigurePopupComponent } from './work/form-list/form-viewer/category-configure-popup/category-configure.popup.component';

@NgModule({
  declarations: [
    AuditLogListComponent,
    CareEventDeletePopupComponent,
    CareEventListComponent,
    CareEventPopupComponent,
    CategoryConfigurePopupComponent,
    ConceptListComponent,
    ConceptPopupComponent,
    ConfigListComponent,
    ConditionListComponent,
    ConditionDeletePopupComponent,
    ConditionPopupComponent,
    ContactDetailListComponent,
    CustomAttributeListComponent,
    CustomAttributeAddPopupComponent,
    CustomAttributeEditPopupComponent,
    CustomAttributeDeletePopupComponent,
    DatasetCategoryListComponent,
    DatasetCategoryDeletePopupComponent,
    DatasetCategoryElementPopupComponent,
    DatasetCategoryElementListComponent,
    DatasetCategoryElementDeletePopupComponent,
    DatasetListComponent,
    DatasetElementDeletePopupComponent,
    DatasetElementListComponent,
    DatasetElementSelectPopupComponent,
    DatasetElementSubDeletePopupComponent,
    DatasetElementSubListComponent,
    DatasetElementSubPopupComponent,    
    EncounterTypeListComponent,
    FacilityListComponent, 
    FacilityPopupComponent,
    FacilityDeletePopupComponent,
    FormListComponent,
    FormDeletePopupComponent,
    FormPopupComponent,
    FormViewerComponent,
    GenericDeletePopupComponent,
    HolidayListComponent,
    HolidayDeletePopupComponent,
    HolidayPopupComponent,
    ImportMeddraPopupComponent,
    LabResultListComponent,
    LabTestListComponent,
    LabTestSelectPopupComponent,
    LandingComponent,
    MeddraListComponent,
    MedicineListComponent,
    MedicationPopupComponent,
    MedicationDeletePopupComponent,
    ReportMetaViewComponent,
    RoleListComponent,
    PasswordResetPopupComponent,
    SelectionItemPopupComponent,
    UserAddPopupComponent,
    UserDeletePopupComponent,
    UserListComponent,
    UserFacilityPopupComponent,
    UserRolePopupComponent,
    UserUpdatePopupComponent,
    WorkPlanListComponent
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
    CareEventPopupComponent,
    CareEventDeletePopupComponent,
    CategoryConfigurePopupComponent,
    ConceptPopupComponent,
    ConditionPopupComponent,
    ConditionDeletePopupComponent,
    CustomAttributeAddPopupComponent,
    CustomAttributeEditPopupComponent,
    CustomAttributeDeletePopupComponent,
    DatasetCategoryDeletePopupComponent,
    DatasetCategoryElementPopupComponent,
    DatasetCategoryElementDeletePopupComponent,
    DatasetElementDeletePopupComponent,
    DatasetElementSelectPopupComponent,
    DatasetElementSubDeletePopupComponent,
    DatasetElementSubPopupComponent,    
    FacilityPopupComponent,
    FacilityDeletePopupComponent,
    GenericDeletePopupComponent,
    HolidayDeletePopupComponent,
    HolidayPopupComponent,
    ImportMeddraPopupComponent,
    LabTestSelectPopupComponent,
    MedicationPopupComponent,
    MedicationDeletePopupComponent,
    PasswordResetPopupComponent,
    SelectionItemPopupComponent,
    UserAddPopupComponent,
    UserDeletePopupComponent,
    UserFacilityPopupComponent,
    UserRolePopupComponent,
    UserUpdatePopupComponent
  ]  
})
export class AdministrationModule { }
