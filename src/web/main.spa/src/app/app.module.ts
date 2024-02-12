import { NgModule, ErrorHandler } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { 
  PerfectScrollbarModule, 
  PERFECT_SCROLLBAR_CONFIG, 
  PerfectScrollbarConfigInterface
} from 'ngx-perfect-scrollbar';

import { rootRouterConfig } from './app.routing';
import { SharedModule } from './shared/shared.module';
import { AppComponent } from './app.component';
import { APP_INITIALIZER } from '@angular/core';
import { UserIdleModule } from 'angular-user-idle';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ErrorHandlerService } from './shared/services/error-handler.service';
import { DatePipe } from '@angular/common';
import { WebcamModule } from 'ngx-webcam';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { HttpInterceptorExtension } from './shared/interceptors/http.interceptor.extension';
import { QuillModule } from 'ngx-quill';
import { PendingChangesGuard } from './shared/guards/component-can-deactive';

// AoT requires an exported function for factories
export function HttpLoaderFactory(httpClient: HttpClient) {
  return new TranslateHttpLoader(httpClient);
}

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    SharedModule,
    HttpClientModule,
    PerfectScrollbarModule,
    WebcamModule,
    QuillModule.forRoot(),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    RouterModule.forRoot(rootRouterConfig, { useHash: false, onSameUrlNavigation: 'reload', relativeLinkResolution: 'legacy' }),
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production }),
    UserIdleModule.forRoot({idle: 600, timeout: 300, ping: 120})
  ],
  declarations: [
    AppComponent
  ],
  providers: [
    DatePipe,
    PendingChangesGuard,
    { provide: HTTP_INTERCEPTORS, useClass: HttpInterceptorExtension, multi: true },
    { provide: ErrorHandler, useClass: ErrorHandlerService },
    { provide: PERFECT_SCROLLBAR_CONFIG, useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }