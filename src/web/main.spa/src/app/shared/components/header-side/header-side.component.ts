import { Component, OnInit, Renderer2, AfterViewInit } from '@angular/core';
import { ThemeService } from '../../services/theme.service';
import { LayoutService } from '../../services/layout.service';
import { TranslateService } from '@ngx-translate/core';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { _routes } from 'app/config/routes';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { UserProfilePopupComponent } from 'app/views/security/user-profile/user-profile.popup.component';
import { PwaService } from 'app/shared/services/pwa.service';
import { AboutPopupComponent } from '../about/about.popup.component';
import { ConfigService } from 'app/shared/services/config.service';
import { MetaService } from 'app/shared/services/meta.service';

@Component({
  selector: 'app-header-side',
  styles: [`
    .error-status { color: red; }
    .connected-status { color: green; }
    .checking-status { color: black; } 
  `],  
  templateUrl: './header-side.template.html'
})
export class HeaderSideComponent implements OnInit, AfterViewInit {
  
  viewModel: ViewModel = new ViewModel();
  
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

  public egretThemes;
  public layoutConf:any;

  pharmadexLink = '';

  constructor(
    private themeService: ThemeService,
    private layout: LayoutService,
    public translate: TranslateService,
    private renderer: Renderer2,
    private accountService: AccountService,
    protected configService: ConfigService,
    protected metaService: MetaService,
    public pwaService: PwaService,
    private _router: Router,
    protected dialog: MatDialog,
  ) {

  }
  
  ngOnInit() {
    this.egretThemes = this.themeService.egretThemes;
    this.layoutConf = this.layout.layoutConf;

    if (localStorage.getItem('locale')) {  
      const browserLang = localStorage.getItem('locale');
      this.translate.use(browserLang);
      this.currentLang = browserLang == 'en' ? this.availableLangs[0] : this.availableLangs[1];
    } 
    else {
      this.translate.use(this.currentLang.code);
    }     
  }
  
  ngAfterViewInit(): void {
    let self = this;
    self.loadPharmadexLink();
  }

  setLang(lng) {
    this.currentLang = lng;
    this.translate.use(lng.code);
    localStorage.setItem('locale', lng.code); 
  }
  
  changeTheme(theme) {
    // this.themeService.changeTheme(theme);
  }

  toggleSidenav() {
    if(this.layoutConf.sidebarStyle === 'closed') {
      return this.layout.publishLayoutChange({
        sidebarStyle: 'full'
      })
    }
    this.layout.publishLayoutChange({
      sidebarStyle: 'closed'
    })
  }

  toggleCollapse() {
    // compact --> full
    if(this.layoutConf.sidebarStyle === 'compact') {
      return this.layout.publishLayoutChange({
        sidebarStyle: 'full',
        sidebarCompactToggle: false
      }, {transitionClass: true})
    }

    // * --> compact
    this.layout.publishLayoutChange({
      sidebarStyle: 'compact',
      sidebarCompactToggle: true
    }, {transitionClass: true})

  }

  onSearch(e) {
    //   console.log(e)
  }

  logout(): void {
    let self = this;
    self.accountService.removeToken();
    self._router.navigate([_routes.security.login])
  }

  refresh(): void {
    window.location.reload();
  }

  refreshMeta(): void {
    let self = this;
    self.viewModel.checking = true;
    self.viewModel.refreshError = false;
    self.metaService.refresh()
      .subscribe(result => {
        self.viewModel.checking = false;
        self.viewModel.refreshError = false;
      }, error => {
        self.viewModel.checking = false;
        self.viewModel.refreshError = true;
      });
  }   

  installPwa(): void {
    this.pwaService.promptEvent.prompt();
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

  loadPharmadexLink(): void {
    let self = this;
    self.configService.getConfigIdentifier(8)
      .subscribe(result => {
        self.pharmadexLink = result.configValue
      });
  }

  navigateToPharmadex(): void {
    window.location.href = "https://www.openrims.org";
  }

  openProfilePopup() {
    let self = this;
    let title = 'User profile';
    let dialogRef: MatDialogRef<any> = self.dialog.open(UserProfilePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
      })
  }  
}

class ViewModel {
  refreshError: boolean = false;
  checking: boolean = false;
}