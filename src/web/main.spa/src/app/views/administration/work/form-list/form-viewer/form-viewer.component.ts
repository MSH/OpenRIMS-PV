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
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaWidgetDetailModel } from 'app/shared/models/meta/meta-widget.detail.model';
import { NavigationService } from 'app/shared/services/navigation.service';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { MetaFormExpandedModel } from 'app/shared/models/meta/meta-form.expanded.model';
import { FormPopupComponent } from '../form-popup/form.popup.component';
import { GenericDeletePopupComponent } from 'app/views/administration/shared/generic-delete-popup/generic-delete.popup.component';
import { CategoryConfigurePopupComponent } from './category-configure-popup/category-configure.popup.component';

@Component({
  templateUrl: './form-viewer.component.html',
  styles: [`
    .mainButton { display: flex; justify-content: flex-end; button { margin-left: auto; } }  
  `],  
  animations: egretAnimations
})
export class FormViewerComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected navigationService: NavigationService,
    protected eventService: EventService,
    protected metaFormService: MetaFormService,
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
        this.initialisetFormView();
      }
    });    
  }

  navigationSubscription;
  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  id: number;
  metaForm: MetaFormExpandedModel;

  leftWidgetList: MetaWidgetDetailModel[] = [];
  rightWidgetList: MetaWidgetDetailModel[] = [];

  ngOnInit() {
    const self = this;
    self.initialisetFormView();
  }

  // Force an event to refresh the page if the paramter changes (but not the route)
  initialisetFormView(): void {
    // Set default values and re-fetch any data you need.
    const self = this;

    self.id = +self._activatedRoute.snapshot.paramMap.get('formid');
    if(self.accountService.hasToken()) {
      self.loadData();
    }
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormViewerComponent.name);
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaFormService.getMetaForm(self.id, 'expanded')
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.metaForm = result;
      }, error => {
        self.handleError(error, "Error fetching meta form");
      });
  }

  openFormPopUp() {
    let self = this;
    let title = 'Update Form';
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId: self.id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.initialisetFormView();
      })
  }  

  openDeletePopUp() {
    let self = this;
    let title = 'Delete Page';
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { id: self.id, type: 'MetaForm', title: title, name: self.metaForm.formName }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.navigationService.routeToAdminLanding();
      })
  }

  // openDeleteWidgetPopUp(id: number, widgetName: string) {
  //   let self = this;
  //   let title = 'Delete Widget';
  //   let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
  //     width: '720px',
  //     disableClose: true,
  //     data: { parentId: self.id, id: id, type: 'MetaWidget', title: title, name: widgetName }
  //   })
  //   dialogRef.afterClosed()
  //     .subscribe(res => {
  //       if(!res) {
  //         // If user press cancel
  //         return;
  //       }
  //       self.initialiseReport();
  //     })
  // }

  openCategoryConfigurePopup(id: number) {
    let self = this;
    let title = id == 0 ? 'Add Category' : 'Update Category';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CategoryConfigurePopupComponent, {
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
        self.initialisetFormView();
      })
  }
}
