import { InjectionToken } from "@angular/core"

export class AppConfig {
    apiURL: string
    apiURLBase: string
    appName: string
    countryISOCode: string
  }
   
  export let APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG')