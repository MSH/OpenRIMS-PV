import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './shared/components/layouts/admin-layout/admin-layout.component';
import { AuthLayoutComponent } from './shared/components/layouts/auth-layout/auth-layout.component';
import { AuthGuard } from './shared/services/auth/auth.guard';

export const rootRouterConfig: Routes = [
  { path: '', redirectTo: 'security/landing', pathMatch: 'full' },
  { path: '', 
    component: AuthLayoutComponent, 
    children: [ 
      { path: 'public', loadChildren: () => import('./views/public/public.module').then(m => m.PublicModule), data: { title: 'Public', breadcrumb: 'PUBLIC'} },
      { path: 'security', loadChildren: () => import('./views/security/security.module').then(m => m.SessionsModule), data: { title: 'Security'} }
    ]
  },
  { path: '', 
    component: AdminLayoutComponent, canActivate: [AuthGuard],
    children: [
      { path: 'error', loadChildren: () => import('./views/error/error.module').then(m => m.ErrorModule), data: { title: 'Error', breadcrumb: 'ERROR'} },
      { path: 'clinical', loadChildren: () => import('./views/clinical/clinical.module').then(m => m.ClinicalModule), data: { title: 'Clinical Portal', breadcrumb: 'Clinical Portal'} },
      { path: 'analytical', loadChildren: () => import('./views/analytical/analytical.module').then(m => m.AnalyticalModule), data: { title: 'Analytical Portal', breadcrumb: 'Analytical Portal'} },
      { path: 'reports', loadChildren: () => import('./views/reports/reports.module').then(m => m.ReportsModule), data: { title: 'Reports Portal', breadcrumb: 'Reports Portal'} },
      { path: 'information', loadChildren: () => import('./views/information/information.module').then(m => m.InformationModule), data: { title: 'Information Portal', breadcrumb: 'Information Portal'} },
      { path: 'administration', loadChildren: () => import('./views/administration/administration.module').then(m => m.AdministrationModule), data: { title: 'Administration Portal', breadcrumb: 'Administration Portal'} }
    ]
  },
  { path: '**', redirectTo: 'error/404' }
];

