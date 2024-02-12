import { Component, OnInit, AfterViewInit, Renderer2 } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router, NavigationEnd, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';

import { RoutePartsService } from "./shared/services/route-parts.service";
// import { ThemeService } from './shared/services/theme.service';

import { filter } from 'rxjs/operators';
import { NavigationService } from './shared/services/navigation.service';
// import { LayoutService } from './shared/services/layout.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, AfterViewInit {
  appTitle = 'OpenRIMS-PVM';
  pageTitle = '';

  constructor(
    public title: Title, 
    private router: Router, 
    private activeRoute: ActivatedRoute,
    private routePartsService: RoutePartsService,
    private navigationService: NavigationService
    // private themeService: ThemeService,
    // private layout: LayoutService,
    // private renderer: Renderer2
  ) { }

  ngOnInit() {
    this.changePageTitleAndPortal();
  }

  ngAfterViewInit() {
    this.changeMenu();
  }

  changePageTitleAndPortal() {
    this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe((routeChange) => {
      console.log('route changed');
      var routeParts = this.routePartsService.generateRouteParts(this.activeRoute.snapshot);
      if (!routeParts.length)
        return this.title.setTitle(this.appTitle);

      // set current portal
      this.navigationService.setCurrentPortal(routeParts[1].title);

      // set current menu structure
      this.navigationService.publishNavigationChange(`${routeParts[1].url}-menu`);

      // Extract title from parts;
      this.pageTitle = routeParts
                      .reverse()
                      .map((part) => part.title )
                      .reduce((partA, partI) => {return `${partA} > ${partI}`});
      this.pageTitle += ` | ${this.appTitle}`;
      this.title.setTitle(this.pageTitle);
    });
  }

  changeMenu() {
  }
  
}
