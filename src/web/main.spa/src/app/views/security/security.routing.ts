
import { Routes } from "@angular/router";

import { ForgotPasswordComponent } from "./forgot-password/forgot-password.component";
import { LockscreenComponent } from "./lockscreen/lockscreen.component";
import { NotFoundComponent } from "./not-found/not-found.component";
import { LoginComponent } from './login/login.component';
import { LandingComponent } from "./landing/landing.component";

export const SecurityRoutes: Routes = [
  {
    path: "landing",
    component: LandingComponent,
    data: { title: "Landing" }
  },
  {
    path: "login",
    component: LoginComponent,
    data: { title: "Login" }
  },
  {
    path: "forgot-password",
    component: ForgotPasswordComponent,
    data: { title: "Forgot password" }
  },
  {
    path: "lockscreen",
    component: LockscreenComponent,
    data: { title: "Lockscreen" }
  },
  {
    path: "404",
    component: NotFoundComponent,
    data: { title: "Not Found" }
  }
];
