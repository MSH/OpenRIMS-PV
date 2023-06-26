import { Injectable } from "@angular/core";
import { SwUpdate } from "@angular/service-worker";

@Injectable({ providedIn: 'root' })
export class PwaService {
  public updateRequired = false;
  public promptEvent: any;

  constructor(private swUpdate: SwUpdate) {
    swUpdate.available.subscribe(event => {
      this.updateRequired = true;
    });

    window.addEventListener('beforeinstallprompt', event => {
      this.promptEvent = event;
    });    
  }
}