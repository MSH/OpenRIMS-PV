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
import { NavigationService } from 'app/shared/services/navigation.service';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { MetaFormExpandedModel } from 'app/shared/models/meta/meta-form.expanded.model';
import { FormPopupComponent } from '../form-popup/form.popup.component';
import { GenericDeletePopupComponent } from 'app/views/administration/shared/generic-delete-popup/generic-delete.popup.component';
import { CategoryConfigurePopupComponent } from './category-configure-popup/category-configure.popup.component';
import { CategoryAttributeConfigurePopupComponent } from './category-attribute-configure-popup/category-attribute-configure.popup.component';
import { CategoryDeletePopupComponent } from './category-delete-popup/category-delete.popup.component';

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

  viewModel: ViewModel = new ViewModel();  

  navigationSubscription;
  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  ngOnInit() {
    const self = this;
    self.initialisetFormView();
  }

  // Force an event to refresh the page if the paramter changes (but not the route)
  initialisetFormView(): void {
    // Set default values and re-fetch any data you need.
    const self = this;

    self.viewModel.formId = +self._activatedRoute.snapshot.paramMap.get('formid');
    if(self.accountService.hasToken()) {
      self.fetchView();
    }
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormViewerComponent.name);
  }

  fetchView(): void {
    let self = this;
    self.setBusy(true);
    self.metaFormService.getMetaForm(self.viewModel.formId, 'expandedwithunmappedattributes')
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.viewModel.metaForm = result;
        self.CLog(result, 'result');
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
      data: { formId: self.viewModel.formId, title: title, payload: self.viewModel.metaForm }
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

  openDeleteFormPopUp() {
    let self = this;
    let title = 'Delete Form';
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { id: self.viewModel.formId, type: 'MetaForm', title: title, name: self.viewModel.metaForm.formName }
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

  openCategoryConfigurePopup(category: any) {
    let self = this;
    let title = category == null ? 'Add Category' : 'Update Category';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CategoryConfigurePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { metaFormId: self.viewModel.formId, metaCategoryId: category == null ? 0 : category.id, title: title, payload: category }
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

  openDeleteCategoryPopUp(category: any) {
    let self = this;
    let title = 'Delete Category';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CategoryDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { metaFormId: self.viewModel.formId, metaCategoryId: category.id, title: title, payload: category }
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

  openCategoryAttributeConfigurePopup(categoryAttribute: any) {
    let self = this;
    let title = !categoryAttribute.selected ? 'Add Category Attribute' : 'Update Category Attribute';
    let dialogRef: MatDialogRef<any> = self.dialog.open(CategoryAttributeConfigurePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { metaFormId: self.viewModel.formId, metaCategoryId: self.viewModel.metaForm.categories[self.viewModel.currentStep].id, metaCategoryAttributeId: !categoryAttribute.selected ? 0 : categoryAttribute.id, title: title, payload: categoryAttribute }
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

  deleteAttribute(categoryAttribute: any) {
    let self = this;
    self.setBusy(true);

    self.metaFormService.deleteMetaCategoryAttribute(self.viewModel.formId, self.viewModel.metaForm.categories[self.viewModel.currentStep].id, categoryAttribute.id)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("Attribute deleted successfully", "Delete Attribute");
      self.initialisetFormView();
    }, error => {
      this.handleError(error, "Error deleting attribute");
    });    
  }

  filterSelectedAttributes(selected: boolean, attributes: any[]): any[] {
    return attributes.filter(a => a.selected == selected);
  }

  nextStep(): void {
    this.viewModel.currentStep ++;
  }

  previousStep(): void {
    this.viewModel.currentStep --;
  }  
}

class ViewModel {
  formId: number;
  metaForm: MetaFormExpandedModel;
  currentStep = 0;
}
