import { environment } from '../../../environments/environment';
import { EventService } from '../services/event.service';
import { _events } from '../../config/events';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { HttpClient, HttpRequest, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { Moment } from 'moment';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { APP_CONFIG, AppConfig } from 'app/app.config';

const moment =  _moment;

@Injectable()
export class BaseService {
    private _baseUrl: string;
    private _baseUrlBase: string;
    protected apiController: string;
    
    private JwtObject: JwtModel;
    
    public lastAction: Date;
    public storeLocal: boolean;
    public refreshingToken: boolean;
    public eulaAcceptanceRequired: boolean;
    public allowDatasetDownload: boolean;

    roles: string[] = [];
    facilities: string[] = [];

    constructor(
        protected httpClient: HttpClient,
        protected eventService: EventService,
        @Inject(APP_CONFIG) config: AppConfig) {
        let self = this;
        console.log('got config' + config);
        self.JwtObject = new JwtModel();
        self._baseUrl = config.apiURL;
        self._baseUrlBase = config.apiURLBase;
        self.loadToken();
        self.lastAction = new Date();
    }

    public ping() {
      return this.httpClient.get(`${this._baseUrl}/accounts/ping`);
    }

    public isAuthenticated() {
        return this.httpClient.get(`${this._baseUrl}${this.apiController}/Authenticated`);
    }

    public Get<T>(address: string, header: string, parameters: ParameterKeyValueModel[]): Observable<unknown> {
        
        let parameterOutput = '';
        if (parameters.length > 1) {
            parameters.forEach(element => parameterOutput += element.key == parameters[0].key ? '?' + element.key + '=' + element.value : '&' + element.key + '=' + element.value);
        }
        if (parameters.length == 1) {
            parameterOutput = `/${parameters[0].value}`;
        }

        if(address == '') {
            address = this.apiController;
        }

        return this.httpClient.get<T>(`${this._baseUrl}${address}${parameterOutput}`, { headers: { 'Accept': header } })
            .pipe(catchError(error => Promise.reject(error)));
    }

    public GetByAddress<T>(address: string, header: string): Observable<unknown> {

      return this.httpClient.get<T>(`${this._baseUrlBase}${address}`, { headers: { 'Accept': header } })
          .pipe(catchError(error => Promise.reject(error)));
    }

    public Post<T>(address: string, data: any): Observable<unknown> {
        console.log(`${this._baseUrl}${this.apiController}/${address}`);

        return this.httpClient.post<T>(`${this._baseUrl}${this.apiController}/${address}`, data, { headers: { 'Content-Type': 'application/json' } })
            .pipe(catchError(error => Promise.reject(error)));
    }

    public PostFile<T>(address: string, data: any): Observable<unknown> {
      return this.httpClient.post<T>(`${this._baseUrl}${this.apiController}/${address}`, data)
          .pipe(catchError(error => Promise.reject(error)));
    }

    public Put<T>(address: string, data: any): Observable<unknown> {
        return this.httpClient.put<T>(`${this._baseUrl}${this.apiController}/${address}`, data, { headers: { 'Content-Type': 'application/json' } })
            .pipe(catchError(error => Promise.reject(error)));
    }

    public Delete<T>(address: string): Observable<unknown> {
      return this.httpClient.delete<T>(`${this._baseUrl}${this.apiController}/${address}`)
          .pipe(catchError(error => Promise.reject(error)));
    }

    public CLog(object: any, title: string = undefined) {
        if (!environment.production)
            console.log({ title: title, object });
    }

    public Download(address: string, header: string, parameters: ParameterKeyValueModel[]): Observable<unknown> {
      let parameterOutput = '';
      if (parameters.length > 1) {
          parameters.forEach(element => parameterOutput += element.key == parameters[0].key ? '?' + element.key + '=' + element.value : '&' + element.key + '=' + element.value);
      }
      if (parameters.length == 1) {
          parameterOutput = `/${parameters[0].value}`;
      }
      
      if(address == '') {
          address = this.apiController;
      }

      return this.httpClient.request(new HttpRequest(
        'GET',
        `${this._baseUrl}${address}${parameterOutput}`,
        null,
        {
          headers: new HttpHeaders( { 'Accept': header }),
          reportProgress: true,
          responseType: 'blob'
        }));          
    }    

    private loadToken() {
      let self = this;
      if (!self.hasToken()) {
        var sessionToken = sessionStorage.getItem("jwttoken");
        var localToken = localStorage.getItem("jwttoken");

        if (sessionToken !== null && localToken !== null) {
            self.clearStorage();
        }

        if (sessionToken !== null) {
            self.storeLocal = false;
            self.JwtObject = new JwtModel(JSON.parse(sessionToken));
        } else if (localToken !== null) {
            self.storeLocal = true;
            self.JwtObject = new JwtModel(JSON.parse(localToken));
        }

        self.roles = self.getTokenRoles();
        self.facilities = self.getTokenFacilities();
      }
    }

    public hasToken(): boolean {
        return this.getToken() != undefined;
    }

    public getToken(): string {
      return this.JwtObject.token;
    }

    public getEmail(): string {
      return this.JwtObject.payload.email;
    }

    public getName(): string {
      return this.JwtObject.payload.given_name;
    }

    public getUsername(): string {
      return this.JwtObject.payload.sub;
    }

    public getUniquename(): number {
      return +this.JwtObject.payload.unique_name;
    }

    public hasRole(role: string) : boolean {
      if(this.roles == undefined) {
        return false;
      }
      return (this.roles.indexOf(role) > -1);
    }

    public getUsertype(): string {
      return this.JwtObject.payload.ut;
    }

    public setSessionToken(jwtObject: any) {
      let self = this;
      self.storeLocal = false;
      self.JwtObject = new JwtModel(jwtObject);
      self.JwtObject.tokenDate = new Date();
      self.eulaAcceptanceRequired = self.JwtObject.eulaAcceptanceRequired;
      self.allowDatasetDownload = self.JwtObject.allowDatasetDownload;
      sessionStorage.setItem("jwttoken", JSON.stringify(jwtObject));
      self.eventService.broadcast(_events.updated_token_event);
      self.roles = self.getTokenRoles();
      self.facilities = self.getTokenFacilities();
      self.eventService.broadcast(_events.roles_refreshed_event);
    }

    private getTokenFacilities(): string[] {
      for (let [key, value] of Object.entries(this.JwtObject.payload)) {
        if (key === 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier') {
          if(Array.isArray(value)){
            return value as string[];
          }
          let retValue = [];
          retValue.push(value);
          return retValue;
        }
      }      
    }

    private getTokenRoles(): string[] {
      for (let [key, value] of Object.entries(this.JwtObject.payload)) {
        if (key === 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role') {
          if(Array.isArray(value)){
            return value as string[];
          }
          let retValue = [];
          retValue.push(value);
          return retValue;
        }
      }      
    }

    public updateToken(jwtObject: any) {
      let self = this;
      if (self.storeLocal) self.setLocalToken(jwtObject);
      else self.setSessionToken(jwtObject);
    }

    private setLocalToken(jwtObject: any) {
      let self = this;
      self.storeLocal = true;
      self.JwtObject = new JwtModel(jwtObject);
      self.JwtObject.tokenDate = new Date();
      localStorage.setItem("jwttoken", JSON.stringify(jwtObject));
      self.eventService.broadcast(_events.updated_token_event);
      self.roles = self.getTokenRoles();
      self.facilities = self.getTokenFacilities();
      self.eventService.broadcast(_events.roles_refreshed_event);
    }

    public removeToken() {
      let self = this;
      self.JwtObject = new JwtModel();
      self.clearStorage();
      self.storeLocal = false;
      self.eventService.broadcast(_events.remove_token_event);
    }

    private saveStorage(key: string, object: any): void {
      let self = this;
      if (self.storeLocal) localStorage.setItem(key, JSON.stringify(object));
      else sessionStorage.setItem(key, JSON.stringify(object));
    }

    private readStorage(key: string): any {
      let self = this;
      if (self.storeLocal) return JSON.parse(localStorage.getItem(key));
      else return JSON.parse(sessionStorage.getItem(key));
    }

    private removeStorage(key: string): void {
      let self = this;
      if (self.storeLocal) localStorage.removeItem(key);
      else sessionStorage.removeItem(key);
    }

    private hasStorage(key: string): boolean {
      let self = this;
      if (self.storeLocal) return localStorage.getItem(key) !== null;
      else return sessionStorage.getItem(key) !== null;
    }

    private clearStorage(): void {
      let self = this;
      if (self.storeLocal) localStorage.clear();
      else sessionStorage.clear();
    }

    public refreshToken() {
      this.eventService.broadcast(_events.refresh_token_event);
      console.log(this.JwtObject.token);
      console.log(this.JwtObject.refreshToken);
      return this.Post(`refreshtoken`, { accessToken: this.JwtObject.token, refreshToken: this.JwtObject.refreshToken });
    }

    public verifyRefreshTokenAction() {
      let self = this;
      if (self.hasToken()) {
        if (new Date() > self.JwtObject.slideDate()) {
          if (self.refreshingToken) return;
          self.refreshingToken = true;
          self.refreshToken()
            .pipe(finalize(() => { self.refreshingToken = false; } ))
            .subscribe(
              result => { self.updateToken(result); },
              error => { self.removeToken(); });
        }
      }
    }

    /**
     * transform all moments in model to an ISO 8601 date format
     * @param model
     */    
    public transformModelForDate(model: any) : any {
      let shallowCopy = Object.assign({}, model);
      Object.keys(shallowCopy).forEach((key, index) => {
        if(moment.isMoment(shallowCopy[key])) {
          shallowCopy[key] = shallowCopy[key].format("YYYY-MM-DD");
        };
        if((typeof shallowCopy[key] === 'object' && shallowCopy[key] != null)) {
          let attributes = shallowCopy[key];
          Object.keys(attributes).forEach((key, index) => {
            if(moment.isMoment(attributes[key])) {
              attributes[key] = attributes[key].format("YYYY-MM-DD");
            }
          })
        };
        if((Array.isArray(shallowCopy[key]) && shallowCopy[key] != null)) {
          for (var arrayIndex = 0; arrayIndex < shallowCopy[key].length; arrayIndex++) {
            let attributes = shallowCopy[key][arrayIndex];
            Object.keys(attributes).forEach((key, index) => {
              if(moment.isMoment(attributes[key])) {
                attributes[key] = attributes[key].format("YYYY-MM-DD");
              }
            })
          }          
        };
      })
      return shallowCopy;
    }
}

class JwtModel {
    constructor(object: any = null) {
        let self = this;
        if (object !== null) {
            self.token = object.accessToken.token;
            self.expiresIn = object.accessToken.expiresIn;
            self.refreshToken = object.refreshToken;
            self.eulaAcceptanceRequired = object.eulaAcceptanceRequired;
            self.allowDatasetDownload = object.allowDatasetDownload;
            let baseSplit = self.token.split('.');
            self.header = JSON.parse(self.decode(baseSplit[0]));
            self.payload = JSON.parse(self.decode(baseSplit[1]));
        }
    }

    header: any = {};
    payload: any = {};

    token: string;
    expiresIn: number;
    refreshToken: string;

    eulaAcceptanceRequired: boolean;
    allowDatasetDownload: boolean;

    tokenDate: Date;

    public encode(object: any): string {
        return window.btoa(object);
    }

    public decode(value: string): any {
        return window.atob(value);
    }

    public getIssuedTokenDate(): Date {
        return new Date(this.payload.iat * 1000);
    }

    public getExpireTokenDate(): Date {
        return new Date(this.payload.exp * 1000);
    }

    public slideDate(): Date {
        let slideAmount = (this.getExpireTokenDate().valueOf() - this.getIssuedTokenDate().valueOf()) / 2;
        return new Date(this.getIssuedTokenDate().valueOf() + slideAmount);
    }
}

export function getBaseUrl() {
    return document.getElementsByTagName('base')[0].href;
}