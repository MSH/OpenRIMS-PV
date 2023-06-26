import { Routes } from '@angular/router';
import { PageViewerComponent } from './page-viewer/page-viewer.component';
import { PageListComponent } from './page-list/page-list.component';

export const InformationRoutes: Routes = [
  {
    path: '',
    children: [{
      path: 'pagelist',
      component: PageListComponent,
      data: { title: 'View Page List', breadcrumb: 'Information Viewer' }
    },
    {
      path: 'pageviewer/:id',
      component: PageViewerComponent,
      data: { title: 'View Information Page', breadcrumb: 'Information Viewer' },
      runGuardsAndResolvers: 'always'      
    }]
  }
];
