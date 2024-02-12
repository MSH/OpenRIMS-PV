import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { MetaPageExpandedModel } from 'app/shared/models/meta/meta-page.expanded.model';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaWidgetDetailModel } from 'app/shared/models/meta/meta-widget.detail.model';
import { MetaPageService } from 'app/shared/services/meta-page.service';
import { PageConfigurePopupComponent } from '../shared/page-configure-popup/page-configure.popup.component';
import { GenericDeletePopupComponent } from '../shared/generic-delete-popup/generic-delete.popup.component';
import { NavigationService } from 'app/shared/services/navigation.service';
import { WidgetConfigurePopupComponent } from './widget-configure-popup/widget-configure.popup.component';
import { WidgetMovePopupComponent } from './widget-move-popup/widget-move.popup.component';

@Component({
  templateUrl: './page-viewer.component.html',
  styles: [`
    .mainButton { display: flex; justify-content: flex-end; button { margin-left: auto; } }  
  `],  
  animations: egretAnimations
})
export class PageViewerComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected navigationService: NavigationService,
    protected eventService: EventService,
    protected metaPageService: MetaPageService,
    public accountService: AccountService,
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          //this.setupTable();
      }
    });

    // Force an event to refresh the page if the paramter changes (but not the route)
    this.navigationSubscription = this._router.events.subscribe((e: any) => {
      // If it is a NavigationEnd event re-initalise the component
      if (e instanceof NavigationEnd) {
        this.initialiseReport();
      }
    });    
  }

  navigationSubscription;
  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  id: number;
  metaPage: MetaPageExpandedModel;

  leftWidgetList: MetaWidgetDetailModel[] = [];
  rightWidgetList: MetaWidgetDetailModel[] = [];

  ngOnInit() {
    const self = this;
    self.initialiseReport();
  }

  // Force an event to refresh the page if the paramter changes (but not the route)
  initialiseReport(): void {
    // Set default values and re-fetch any data you need.
    const self = this;

    self.id = +self._activatedRoute.snapshot.paramMap.get('id');
    if(self.accountService.hasToken()) {
      self.loadData();
    }
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(PageViewerComponent.name);
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaPageService.getMetaPage(self.id, 'expanded')
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.metaPage = result;

        self.leftWidgetList = [];
        self.leftWidgetList.push(self.metaPage.widgets.find(w => w.widgetLocation == 'TopLeft'));
        self.leftWidgetList.push(self.metaPage.widgets.find(w => w.widgetLocation == 'MiddleLeft'));
        self.leftWidgetList.push(self.metaPage.widgets.find(w => w.widgetLocation == 'BottomLeft'));

        self.rightWidgetList = [];
        self.rightWidgetList.push(self.metaPage.widgets.find(w => w.widgetLocation == 'TopRight'));
        self.rightWidgetList.push(self.metaPage.widgets.find(w => w.widgetLocation == 'MiddleRight'));
        self.rightWidgetList.push(self.metaPage.widgets.find(w => w.widgetLocation == 'BottomRight'));
      }, error => {
        self.handleError(error, "Error fetching meta page");
      });
  }

  openPageConfigurePopup() {
    let self = this;
    let title = 'Update Page';
    let dialogRef: MatDialogRef<any> = self.dialog.open(PageConfigurePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { metaPageId: self.id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        self.initialiseReport();
      })
  }

  openDeletePopUp() {
    let self = this;
    let title = 'Delete Page';
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { id: self.id, type: 'MetaPage', title: title, name: self.metaPage.pageName }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.navigationService.routeToPublisherHome();
      })
  }

  openDeleteWidgetPopUp(id: number, widgetName: string) {
    let self = this;
    let title = 'Delete Widget';
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { parentId: self.id, id: id, type: 'MetaWidget', title: title, name: widgetName }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.initialiseReport();
      })
  }

  openWidgetConfigurePopup(id: number) {
    let self = this;
    let title = id == 0 ? 'Add Widget' : 'Update Widget';
    let dialogRef: MatDialogRef<any> = self.dialog.open(WidgetConfigurePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { metaPageId: self.id, metaWidgetId: id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        self.initialiseReport();
      })
  }

  openWidgetMovePopup(id: number, widgetName: string) {
    let self = this;
    let title = 'Move Widget';
    let dialogRef: MatDialogRef<any> = self.dialog.open(WidgetMovePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { metaPageId: self.id, metaWidgetId: id, title: title, widgetName }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        self.initialiseReport();
      })
  }
}
