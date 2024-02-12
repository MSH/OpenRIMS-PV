import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AccountService } from '../services/account.service';
import { _routes } from '../../config/routes';

@Injectable()
export class HttpInterceptorExtension implements HttpInterceptor {
    constructor(private router: Router,
        private accountService: AccountService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let self = this;
        self.accountService.lastAction = new Date();
        if (self.accountService.hasToken()) {
            return self.generateAuthothorizedResponse(request, next);
        }
        return next.handle(request);
    }

    generateAuthothorizedResponse(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let self = this;
        let cloneRequest = request.clone({
            headers: request.headers
                .set("Authorization", `Bearer ${self.accountService.getToken()}`)
            //.set('Content-Type', 'application/json')
        });
        return next.handle(cloneRequest).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error.status >= 200 && error.status < 300) {
                    const res = new HttpResponse({
                        body: null,
                        headers: error.headers,
                        status: error.status,
                        statusText: error.statusText,
                        url: error.url
                    });
                    return of(res);
                }
                if (error.status === 401) {
                  self.accountService.removeToken();
                  self.router.navigate([_routes.security.login])
                }                
                let tokenExpired = error.headers.get(`X-Token-Expired`);
                if (tokenExpired === "true") {
                    self.accountService.refreshToken()
                        .toPromise().then(result => {
                            self.accountService.updateToken(result);
                            return self.generateAuthothorizedResponse(request, next);
                        }, e => {
                            self.accountService.removeToken();
                            return self.handleErrors(error);
                        });
                    return throwError(error.error);
                } else {
                    return self.handleErrors(error);
                }
            }) as any);
    }

    handleErrors(error: HttpErrorResponse): Observable<HttpEvent<any>> {
        let self = this;
        // if (error.status > 400) {
        //     switch (error.status) {
        //         case 403: self.router.navigate([_routes.error._403], { state: { error: error.error } }); break;
        //         case 404: self.router.navigate([_routes.error._404], { state: { error: error.error } }); break;
        //         case 500: self.router.navigate([_routes.error._500], { state: { error: error.error } }); break;
        //         case 501: self.router.navigate([_routes.error._501], { state: { error: error.error } }); break;
        //         default: self.router.navigate([_routes.error.general]); break;
        //     }
        // }
        return throwError(error.error || error);
    }
}
