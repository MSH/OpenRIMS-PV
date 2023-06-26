import { CanDeactivate } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { PopupService } from '../services/popup.service';

export interface ComponentCanDeactivate {
  canDeactivate: () => boolean | Observable<boolean>;
}

@Injectable()
export class PendingChangesGuard implements CanDeactivate<ComponentCanDeactivate> {
  
  constructor(
    protected popupService: PopupService) {
  }
  
  canDeactivate(component: ComponentCanDeactivate): boolean | Observable<boolean> {
    // if there are no pending changes, just allow deactivation; else confirm first
    return component.canDeactivate() ?
      true :
      this.popupService.showConfirmMessage('Press Cancel to go back and save these changes, or OK to lose these changes.', 'WARNING: You have unsaved changes');
  }
}