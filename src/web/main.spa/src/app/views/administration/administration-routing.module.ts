import { Routes } from '@angular/router';
import { ConfigListComponent } from './system/config-list/config-list.component';
import { LabResultListComponent } from './reference/lab-result-list/lab-result-list.component';
import { FacilityListComponent } from './system/facility-list/facility-list.component';
import { LabTestListComponent } from './reference/lab-test-list/lab-test-list.component';
import { MedicineListComponent } from './reference/medicine-list/medicine-list.component';
import { UserListComponent } from './user/user-list/user-list.component';
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
import { FormListComponent } from './work/form-list/form-list.component';
import { DatasetCategoryListComponent } from './work/dataset-list/dataset-category-list/dataset-category-list.component';
import { DatasetCategoryElementListComponent } from './work/dataset-list/dataset-category-element-list/dataset-category-element-list.component';
import { ConceptListComponent } from './reference/concept-list/concept-list.component';
import { CustomAttributeListComponent } from './work/custom-attribute-list/custom-attribute-list.component';
import { DatasetElementSubListComponent } from './work/dataset-element-sub-list/dataset-element-sub-list.component';
import { FormViewerComponent } from './work/form-list/form-viewer/form-viewer.component';

export const ReportRoutes: Routes = [
  {
    path: 'landing',
    component: LandingComponent,
    data: { title: 'Landing', breadcrumb: 'Landing' }
  },
  {
    path: 'auditlog',
    component: AuditLogListComponent,
    data: { title: 'Audit Log', breadcrumb: 'Audit Log' }
  },
  {
    path: 'system/config',
    component: ConfigListComponent,
    data: { title: 'Configuration', breadcrumb: 'Configuration' }
  },
  {
    path: 'system/contactdetail',
    component: ContactDetailListComponent,
    data: { title: 'Contact Detail', breadcrumb: 'Conntact Detail' }
  },
  {
    path: 'system/facility',
    component: FacilityListComponent,
    data: { title: 'Facility Management', breadcrumb: 'Facility Management' }
  },
  {
    path: 'system/holiday',
    component: HolidayListComponent,
    data: { title: 'Public Holidays', breadcrumb: 'Public Holidays' }
  },
  {
    path: 'system/reportmeta',
    component: ReportMetaViewComponent,
    data: { title: 'Report Meta Data', breadcrumb: 'Report Meta Data' }
  },
  {
    path: 'reference/condition',
    component: ConditionListComponent,
    data: { title: 'Condition Groups Management', breadcrumb: 'Condition Groups Management' }
  },
  {
    path: 'reference/labresult',
    component: LabResultListComponent,
    data: { title: 'Tests Results Management', breadcrumb: 'Tests Results Management' }
  },
  {
    path: 'reference/labtest',
    component: LabTestListComponent,
    data: { title: 'Tests and Procedures Management', breadcrumb: 'Tests and Procedures Management' }
  },
  {
    path: 'reference/meddra',
    component: MeddraListComponent,
    data: { title: 'Meddra Management', breadcrumb: 'Meddra Management' }
  },
  {
    path: 'reference/concept',
    component: ConceptListComponent,
    data: { title: 'Active Ingredient Management', breadcrumb: 'Active Ingredient Management' }
  },
  {
    path: 'reference/medicine',
    component: MedicineListComponent,
    data: { title: 'Medicine Management', breadcrumb: 'Medicine Management' }
  },
  {
    path: 'user/user',
    component: UserListComponent,
    data: { title: 'User Management', breadcrumb: 'User Management' }
  },
  {
    path: 'user/role',
    component: RoleListComponent,
    data: { title: 'Role Management', breadcrumb: 'Role Management' }
  },
  {
    path: 'work/attributes',
    component: CustomAttributeListComponent,
    data: { title: 'Custom Attribute Management', breadcrumb: 'Custom Attribute Management' }
  },
  {
    path: 'work/careevent',
    component: CareEventListComponent,
    data: { title: 'Care Event Management', breadcrumb: 'Care Event Management' }
  },
  {
    path: 'work/dataset',
    component: DatasetListComponent,
    data: { title: 'Dataset Management', breadcrumb: 'Dataset Management' }
  },
  {
    path: 'work/datasetcategory/:datasetid',
    component: DatasetCategoryListComponent,
    data: { title: 'Dataset Category Management', breadcrumb: 'Dataset Category Management' }
  },  
  {
    path: 'work/datasetcategoryelement/:datasetid/:datasetcategoryid',
    component: DatasetCategoryElementListComponent,
    data: { title: 'Dataset Category Element Management', breadcrumb: 'Dataset Category Element Management' }
  },  
  {
    path: 'work/datasetelement',
    component: DatasetElementListComponent,
    data: { title: 'Dataset Element Management', breadcrumb: 'Dataset Element Management' }
  },
  {
    path: 'work/datasetelement/:datasetelementid/datasetelementsub',
    component: DatasetElementSubListComponent,
    data: { title: 'Dataset Element Sub Management', breadcrumb: 'Dataset Element Sub Management' }
  },  
  {
    path: 'work/encountertype',
    component: EncounterTypeListComponent,
    data: { title: 'Encounter Type Management', breadcrumb: 'Encounter Type Management' }
  },
  {
    path: 'work/form',
    component: FormListComponent,
    data: { title: 'Form Management', breadcrumb: 'Form Management' }
  },
  {
    path: 'work/form-view/:formid',
    component: FormViewerComponent,
    data: { title: 'Form View', breadcrumb: 'Form View' }
  },
  {
    path: 'work/workplan',
    component: WorkPlanListComponent,
    data: { title: 'Work Plan Management', breadcrumb: 'Work Plan Management' }
  }
];