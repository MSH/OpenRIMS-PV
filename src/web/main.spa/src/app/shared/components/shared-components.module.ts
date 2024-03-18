import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedMaterialModule } from '../shared-material.module';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { SharedPipesModule } from '../pipes/shared-pipes.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedDirectivesModule } from '../directives/shared-directives.module';

// ONLY REQUIRED FOR **SIDE** NAVIGATION LAYOUT
import { HeaderSideComponent } from './header-side/header-side.component';
import { SidebarSideComponent } from './sidebar-side/sidebar-side.component';

// ONLY REQUIRED FOR **TOP** NAVIGATION LAYOUT
import { HeaderTopComponent } from './header-top/header-top.component';
import { SidebarTopComponent } from './sidebar-top/sidebar-top.component';

// ONLY FOR DEMO
import { CustomizerComponent } from './customizer/customizer.component';

// ALWAYS REQUIRED 
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { SidenavComponent } from './sidenav/sidenav.component';
import { FooterComponent } from './footer/footer.component';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { AppComfirmComponent } from '../services/app-confirm/app-confirm.component';
import { AppLoaderComponent } from '../services/app-loader/app-loader.component';
import { ButtonLoadingComponent } from './button-loading/button-loading.component';
import { EgretSidebarComponent, EgretSidebarTogglerDirective } from './egret-sidebar/egret-sidebar.component';
import { BottomSheetShareComponent } from './bottom-sheet-share/bottom-sheet-share.component';
import { ViewErrorPopupComponent } from 'app/views/clinical/form/synchronise/viewerror-popup/viewerror.popup.component';
import { LabPopupComponent } from 'app/views/administration/reference/lab-test-list/lab-popup/lab.popup.component';
import { LabDeletePopupComponent } from 'app/views/administration/reference/lab-test-list/lab-delete-popup/lab-delete.popup.component';
import { LabResultPopupComponent } from 'app/views/administration/reference/lab-result-list/lab-result-popup/lab-result.popup.component';
import { LabResultDeletePopupComponent } from 'app/views/administration/reference/lab-result-list/lab-result-delete-popup/lab-result-delete.popup.component';
import { ConfigPopupComponent } from 'app/views/administration/system/config-list/config-popup/config.popup.component';
import { ContactDetailPopupComponent } from 'app/views/administration/system/contact-detail-list/contact-detail-popup/contact-detail.popup.component';
import { DatasetElementPopupComponent } from 'app/views/administration/work/dataset-element-list/dataset-element-popup/dataset-element.popup.component';
import { DatasetPopupComponent } from 'app/views/administration/work/dataset-list/dataset-popup/dataset.popup.component';
import { EncounterTypePopupComponent } from 'app/views/administration/work/encounter-type-list/encounter-type-popup/encounter-type.popup.component';
import { EncounterTypeDeletePopupComponent } from 'app/views/administration/work/encounter-type-list/encounter-type-delete-popup/encounter-type-delete.popup.component';
import { DatasetDeletePopupComponent } from 'app/views/administration/work/dataset-list/dataset-delete-popup/dataset-delete.popup.component';
import { FormCompletePopupComponent } from 'app/views/clinical/form/form-complete-popup/form-complete.popup.component';
import { PingComponent } from './ping/ping.component';
import { UserProfilePopupComponent } from 'app/views/security/user-profile/user-profile.popup.component';
import { EnrolmentPopupComponent } from 'app/views/clinical/patient/patient-view/enrolment-popup/enrolment.popup.component';
import { DeenrolmentPopupComponent } from 'app/views/clinical/patient/patient-view/deenrolment-popup/deenrolment.popup.component';
import { AttachmentPopupComponent } from 'app/views/clinical/patient/patient-view/attachment-popup/attachment.popup.component';
import { ConfirmPopupComponent } from './popup/confirm.popup.component';
import { ErrorPopupComponent } from './popup/error.popup.component';
import { InfoPopupComponent } from './popup/info.popup.component';
import { ConceptSelectPopupComponent } from './popup/concept-select-popup/concept-select.popup.component';
import { AboutPopupComponent } from './about/about.popup.component';
import { TimeoutComponent } from './timeout/timeout.component';
import { MeddraSelectPopupComponent } from './popup/meddra-select-popup/meddra-select.popup.component';
import { DatasetCategoryPopupComponent } from 'app/views/administration/work/dataset-list/dataset-category-list/dataset-category-popup/dataset-category.popup.component';
import { TaskCommentsPopupComponent } from './popup/task-comments-popup/task-comments.popup.component';
import { OnlineStatusPopupComponent } from './popup/online-status-popup/online-status.popup.component';
import { ChangeTaskStatusPopupComponent } from './popup/change-task-status-popup/change-task-status.popup.component';

const components = [
  HeaderTopComponent,
  HeaderSideComponent,
  BreadcrumbComponent,

  SidebarTopComponent,
  SidebarSideComponent,
  SidenavComponent,
  
  NotificationsComponent,

  AdminLayoutComponent,
  AuthLayoutComponent,
  
  AppComfirmComponent,
  ConfirmPopupComponent,
  ErrorPopupComponent,
  InfoPopupComponent,
  AppLoaderComponent,
  UserProfilePopupComponent,

  // Form popups
  FormCompletePopupComponent,
  
  // Patient view popups
  AttachmentPopupComponent,
  EnrolmentPopupComponent,
  DeenrolmentPopupComponent,

  ConfigPopupComponent,
  ContactDetailPopupComponent,
  DatasetPopupComponent,
  DatasetDeletePopupComponent,
  DatasetCategoryPopupComponent,
  DatasetElementPopupComponent,
  EncounterTypePopupComponent,
  EncounterTypeDeletePopupComponent,
  LabPopupComponent,
  LabDeletePopupComponent,
  LabResultPopupComponent,
  LabResultDeletePopupComponent,

  AboutPopupComponent,
  ChangeTaskStatusPopupComponent,
  ConceptSelectPopupComponent,
  MeddraSelectPopupComponent,
  OnlineStatusPopupComponent,
  TaskCommentsPopupComponent,
  ViewErrorPopupComponent,

  CustomizerComponent,
  ButtonLoadingComponent,
  EgretSidebarComponent,
  FooterComponent,
  EgretSidebarTogglerDirective,
  BottomSheetShareComponent,
  PingComponent,
  TimeoutComponent
]

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    FlexLayoutModule,
    ReactiveFormsModule,
    PerfectScrollbarModule,
    SharedPipesModule,
    SharedDirectivesModule,
    SharedMaterialModule,
    TranslateModule
  ],
  declarations: components,
  entryComponents: [
    AppComfirmComponent, 
    AppLoaderComponent, 
    BottomSheetShareComponent, 
    ConfirmPopupComponent, 
    ErrorPopupComponent, 
    InfoPopupComponent,
    UserProfilePopupComponent,
    
    FormCompletePopupComponent,

    AttachmentPopupComponent,
    EnrolmentPopupComponent,
    DeenrolmentPopupComponent,
    
    ContactDetailPopupComponent,
    ConfigPopupComponent,
    DatasetPopupComponent,
    DatasetDeletePopupComponent,
    DatasetElementPopupComponent,
    DatasetCategoryPopupComponent,
    EncounterTypePopupComponent,
    EncounterTypeDeletePopupComponent,
    LabPopupComponent,
    LabDeletePopupComponent,
    LabResultPopupComponent,
    LabResultDeletePopupComponent,

    AboutPopupComponent,
    ConceptSelectPopupComponent,
    MeddraSelectPopupComponent,
    OnlineStatusPopupComponent,
    TaskCommentsPopupComponent,
    ViewErrorPopupComponent
  ],
  exports: components
})
export class SharedComponentsModule {}