import { Routes } from '@angular/router';
import { SpontaneousComponent } from './spontaneous/spontaneous.component';

export const PublicRoutes: Routes = [
  {
    path: "spontaneous",
    component: SpontaneousComponent,
    data: { title: "Public Spontaneous Report" }
  }
];