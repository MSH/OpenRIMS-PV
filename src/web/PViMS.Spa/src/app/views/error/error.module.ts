import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Error401Component } from './401/error.401.component';
import { Error403Component } from './403/error.403.component';
import { Error404Component } from './404/error.404.component';
import { Error500Component } from './500/error.500.component';
import { ErrorGeneralComponent } from './general/error.general.component';
import { _paths } from '../../config/paths';
import { MatIconModule } from '@angular/material/icon';
import { Error501Component } from './501/error.501.component';
import { SharedModule } from 'app/shared/shared.module';

const routes = [
    { path: `${_paths.errorPath._401}`, component: Error401Component },
    { path: `${_paths.errorPath._403}`, component: Error403Component },
    { path: `${_paths.errorPath._404}`, component: Error404Component },
    { path: `${_paths.errorPath._500}`, component: Error500Component },
    { path: `${_paths.errorPath._501}`, component: Error501Component },
    { path: `${_paths.errorPath.general}`, component: ErrorGeneralComponent }
];

@NgModule({
    declarations: [
        Error401Component,
        Error403Component,
        Error404Component,
        Error500Component,
        Error501Component,
        ErrorGeneralComponent
    ],
    imports: [
        MatIconModule,
        SharedModule,
        RouterModule.forChild(routes),
    ],
    exports: [
        Error401Component,
        Error403Component,
        Error404Component,
        Error500Component,
        Error501Component,
        ErrorGeneralComponent
    ]
})

export class ErrorModule {
}
