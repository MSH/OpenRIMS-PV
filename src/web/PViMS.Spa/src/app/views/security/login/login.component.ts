import { Component, OnInit, ViewChild } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatProgressBar } from '@angular/material/progress-bar';
import { Validators, FormGroup, FormControl } from '@angular/forms';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { _routes } from 'app/config/routes';
import { PopupService } from 'app/shared/services/popup.service';
import { finalize } from 'rxjs/operators';
import { NavigationService } from 'app/shared/services/navigation.service';
import { AcceptEulaPopupComponent } from '../accept-eula/accept-eula.popup.component';
import { environment } from 'environments/environment';

@Component({
  selector: 'app-signin',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  @ViewChild(MatProgressBar) progressBar: MatProgressBar;
  @ViewChild(MatButton) submitButton: MatButton;

  viewModelForm: FormGroup;

  busy: boolean = false;

  constructor(
    protected _router: Router,
    protected accountService: AccountService,
    protected navigationService: NavigationService,
    protected popupService: PopupService,
    protected dialog: MatDialog,
  ) { }

  ngOnInit() {
    this.viewModelForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    })
  }

  login(): void {
    let self = this;
    self.setBusy(true);
    self.accountService.login(self.viewModelForm.value)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.accountService.setSessionToken(result as any);

          if(self.accountService.eulaAcceptanceRequired) {
            self.openEulaPopUp();
          }
          else {
            self.notify("Successfully authenticated!", "Login");
            self.navigationService.determineRouteToLanding();
          }
        }, error => {
          self.handleError(error, "Error logging in");
        });
  }

  openEulaPopUp() {
    let self = this;
    let title = 'Accept End User License Agreement';
    let dialogRef: MatDialogRef<any> = self.dialog.open(AcceptEulaPopupComponent, {
      width: '820px',
      disableClose: true,
      data: { title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          self.accountService.removeToken();
          return;
        }

        self.notify("Successfully authenticated!", "Login");
        self.navigationService.determineRouteToLanding();
      })
  }

  public isBusy(): boolean {
    return this.busy;
  }

  public setBusy(value: boolean): void {
    setTimeout(() => { this.busy = value; });
  }  

  private notify(message: string, action: string) {
    return this.popupService.notify(message, action);
  }

  public CLog(object: any, title: string = undefined) {
    if (!environment.production) {
        console.log({ title: title, object });
    }
  }  

  private handleError(errorObject: any, title: string = "Exception")
  {
    let message = "Unknown error experienced. Please contact your system administrator. ";
    if(errorObject.error) {
      if(Array.isArray(errorObject.error.Message)) {
        message = errorObject.error.Message[0];
      }
      else {
        message = errorObject.error.Message;
      }
    }

    if(!errorObject.error && errorObject.message) {
      if(Array.isArray(errorObject.message)) {
        message = errorObject.message[0];
      }
      else {
        message = errorObject.message;
      }
    }

    if(errorObject.ReferenceCode) {
      message += `Reference Code: ${errorObject.ReferenceCode}`;
    }

    this.showError(message, title);
  }
    
  private showError(errorMessage: any, title: string = "Error") {
    this.popupService.showErrorMessage(errorMessage, title);
  }

  protected showInfo(message: string, title: string = "Info") {
    this.popupService.showInfoMessage(message, title);
  }  

}
