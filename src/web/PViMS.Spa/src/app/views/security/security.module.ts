import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from "@angular/router";
import { SharedMaterialModule } from 'app/shared/shared-material.module';

import { FlexLayoutModule } from '@angular/flex-layout';

// import { CommonDirectivesModule } from './sdirectives/common/common-directives.module';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { LockscreenComponent } from './lockscreen/lockscreen.component';
import { SecurityRoutes } from "./security.routing";
import { NotFoundComponent } from './not-found/not-found.component';
import { LoginComponent } from './login/login.component';
import { SharedComponentsModule } from 'app/shared/components/shared-components.module';
import { SharedModule } from 'app/shared/shared.module';
import { AcceptEulaPopupComponent } from './accept-eula/accept-eula.popup.component';
import { LandingComponent } from './landing/landing.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedMaterialModule,
    SharedComponentsModule,
    SharedModule,
    FlexLayoutModule,
    PerfectScrollbarModule,
    RouterModule.forChild(SecurityRoutes)
  ],
  declarations: [
    ForgotPasswordComponent, 
    LockscreenComponent, 
    NotFoundComponent, 
    LoginComponent,
    AcceptEulaPopupComponent,
    LandingComponent
  ],
  entryComponents: [
    AcceptEulaPopupComponent
  ]
})
export class SessionsModule { }