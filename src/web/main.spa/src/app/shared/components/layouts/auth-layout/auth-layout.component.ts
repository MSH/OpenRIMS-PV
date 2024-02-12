import { Component, OnInit } from '@angular/core';
import { ThemeService } from 'app/shared/services/theme.service';
import { LayoutService } from 'app/shared/services/layout.service';
import { Subscription } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-auth-layout',
  templateUrl: './auth-layout.component.html'
})
export class AuthLayoutComponent implements OnInit {
  private moduleLoaderSub: Subscription;
  private layoutConfSub: Subscription;
  private routerEventSub: Subscription;  

  public  scrollConfig = {}
  public layoutConf: any = {};
  public adminContainerClasses: any = {};

  constructor(
    public themeService: ThemeService,
    public translate: TranslateService,
    private layout: LayoutService
  ) 
  { 
    // Translator init
    translate.setDefaultLang('en');
  }

  ngOnInit() {
    // this.layoutConf = this.layout.layoutConf;
    this.layoutConfSub = this.layout.layoutConf$.subscribe((layoutConf) => {
      this.layoutConf = layoutConf;
      // console.log(this.layoutConf);
      
  });    
  }

}
