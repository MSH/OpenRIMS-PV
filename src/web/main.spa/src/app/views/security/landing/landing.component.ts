import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { _routes } from 'app/config/routes';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AboutPopupComponent } from 'app/shared/components/about/about.popup.component';
import { LayoutService } from 'app/shared/services/layout.service';
import { TranslateService } from '@ngx-translate/core';
import { APP_CONFIG, AppConfig } from 'app/app.config';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.scss']
})
export class LandingComponent implements OnInit {

  logo = '';

  public availableLangs = [{
    name: 'EN',
    code: 'en',
    flag: 'flag-icon-us'
  }, 
  {
    name: 'Mz',
    code: 'mz',
    flag: 'flag-icon-mz'
  },
  {
    name: 'Fr',
    code: 'fr',
    flag: 'flag-icon-fr'
  }]
  currentLang = this.availableLangs[0];  

  constructor(
    protected _router: Router,
    protected dialog: MatDialog,
    protected layout: LayoutService,
    @Inject(APP_CONFIG) config: AppConfig,
    public translate: TranslateService
  ) 
  { 
    let self = this;
    self.logo = `assets/images/site_logo_${config.countryISOCode}.png`;
  }

  ngOnInit() {
  }

  navigateToSpontaneous(): void {
    let self = this;
    self._router.navigate([_routes.public.spontaneous]);
  }

  navigateToLogin(): void {
    let self = this;
    self._router.navigate([_routes.security.login]);
  }

  openAboutPopUp(data: any = {}) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(AboutPopupComponent, {
      width: '920px',
      disableClose: true
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
      })
  } 
  
  setLang(lng) {
    this.currentLang = lng;
    this.translate.use(lng.code);
    localStorage.setItem('locale', lng.code); 
  }  
}
