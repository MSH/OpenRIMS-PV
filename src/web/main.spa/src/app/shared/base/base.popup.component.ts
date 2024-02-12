import { Location } from '../../../../node_modules/@angular/common';
import { environment } from '../../../environments/environment';
import { PopupService } from '../services/popup.service';
import { AccountService } from '../services/account.service';
import { FormGroup, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { AttributeValueModel } from '../models/attributevalue.model';

export class BasePopupComponent {

    protected busy: boolean = false;

    constructor(
        protected _router: Router,
        protected _location: Location,
        protected popupService: PopupService,
        protected accountService: AccountService) {
    }

    public CLog(object: any, title: string = undefined) {
        if (!environment.production) {
            console.log({ title: title, object });
        }
    }

    public CLogFormErrors(viewModelForm: FormGroup) {
      Object.keys(viewModelForm.controls).forEach(key => {

        const controlErrors: ValidationErrors = viewModelForm.get(key).errors;
        if (controlErrors != null) {
          Object.keys(controlErrors).forEach(keyError => {
            console.log('Key control: ' + key + ', keyError: ' + keyError + ', err value: ', controlErrors[keyError]);
          });
        }
      })
    }

    public isBusy(): boolean {
        return this.busy;
    }

    public setBusy(value: boolean): void {
        setTimeout(() => { this.busy = value; });
    }

    protected showError(errorMessage: any, errorCode: string, title: string = "Error") {
        this.popupService.showErrorMessage(errorMessage, errorCode, title);
    }

    protected showInfo(message: string, title: string = "Info") {
        this.popupService.showInfoMessage(message, title);
    }

    protected notify(message: string, action: string) {
        return this.popupService.notify(message, action);
    }

    protected showConfirm(message: string, title: string = "Confirm") {
        this.popupService.showConfirmMessage(message, title);
    }

    protected throwError(errorObject: any, title: string = "Exception") {
        if (errorObject.status == 401) {
            this.showError(errorObject.error.message, errorObject.error.statusCodeType);
        } else {
            this.showError(errorObject.message, title);
        }
    }

    protected handleError(errorObject: any, title: string = "Exception")
    {
      let message = 'Unknown error experienced. Please contact your system administrator. ';
      if(errorObject.message) {
        if(Array.isArray(errorObject.message)) {
          message = errorObject.message[0];
        }
        else {
          message = errorObject.message;
        }
      }
      if(errorObject.Message) {
        if(Array.isArray(errorObject.Message)) {
          message = errorObject.Message[0];
        }
        else {
          message = errorObject.Message;
        }
      }

      let errorCode = '';
      if(errorObject.ReferenceCode) {
        errorCode += `Reference Code: ${errorObject.ReferenceCode}`;
      }

      this.CLog(errorObject, title);
      this.showError(message, errorCode, title);
    }

    public setForm(form: FormGroup, value: any): void {
        form.setValue(value);
    }

    public updateForm(form: FormGroup, value: any): void {
        form.patchValue(value);
    }

    public resetForm(form: FormGroup): void {
        form.reset();
    }

    public markFormGroupTouched(formGroup: FormGroup) {
      (<any>Object).values(formGroup.controls).forEach(control => {
        control.markAsTouched();
  
        if (control.controls) {
          this.markFormGroupTouched(control);
        }
      });
    } 
    
    public getValueOrSelectedValueFromAttribute(attributes: AttributeValueModel[], key: string): string {
      let attribute = attributes.find(a => a.key == key);
      if(attribute?.selectionValue != '') {
        return attribute?.selectionValue;
      }
       return attribute?.value;
    }
}
