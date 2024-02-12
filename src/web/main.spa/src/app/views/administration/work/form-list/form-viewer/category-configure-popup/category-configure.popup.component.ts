import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup, AbstractControl, ValidatorFn, ValidationErrors } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { MetaPageService } from 'app/shared/services/meta-page.service';
import { WidgetListItemModel } from 'app/shared/models/meta/widget-list-item.model';
import { MetaPageDetailModel } from 'app/shared/models/meta/meta-page.detail.model';

@Component({
  templateUrl: './category-configure.popup.component.html',
  styleUrls: ['./category-configure.popup.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class CategoryConfigurePopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: WidgetConfigurePopupData,
    public dialogRef: MatDialogRef<CategoryConfigurePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected metaPageService: MetaPageService,
    protected popupService: PopupService,
    protected accountService: AccountService) 
  { 
    super(_router, _location, popupService, accountService);        
  }

  viewModel: ViewModel = new ViewModel();

  pageList: MetaPageDetailModel[] = [];

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this._formBuilder.group({
      metaWidgetGuid: [''],
      widgetName: ['', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z0-9 ]*')]],
      widgetStatus: ['Unpublished', widgetLocationValidator],
      currentLocation: ['Unassigned'],
      widgetLocation: ['Unassigned', widgetLocationValidator],
      widgetType: ['', Validators.required],
      icon: ['', Validators.required],
      generalContent: [''],
      itemTitle: ['', [Validators.maxLength(100), Validators.pattern('[-a-zA-Z0-9 .,()*]*')]],
      itemSubtitle: ['', [Validators.maxLength(100), Validators.pattern('[-a-zA-Z0-9 .,()*]*')]],
      itemContent: [''],
      itemContentPageId: ['']
    })

  }

  loadDropDowns(): void {
    let self = this;
    self.getPageList();
  }
  
  getPageList(): void {
    let self = this;

    self.metaPageService.getAllMetaPages()
        .subscribe(result => {
          self.pageList = result;
        }, error => {
            self.handleError(error, "Error fetching meta pages");
        });
  }  

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.metaWidgetId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaPageService.getMetaWidget(self.data.metaPageId, self.data.metaWidgetId, 'detail')
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.updateForm(self.itemForm, result);
        self.updateForm(self.itemForm, {generalContent: result.content});

        self.viewModel.selectedIcon = result.icon;
        self.viewModel.contentItems = result.contentItems;
      }, error => {
        this.handleError(error, "Error fetching meta widget");
      });
  }  

  submit() {
    let self = this;

    self.itemForm.get('widgetLocation').updateValueAndValidity();
    self.itemForm.get('widgetStatus').updateValueAndValidity();

    if (self.itemForm.valid) {
      self.setBusy(true);

      self.metaPageService.saveMetaWidget(self.data.metaPageId, self.data.metaWidgetId, self.itemForm.value, self.viewModel.contentItems)
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.notify("Meta widget saved successfully", "Meta Pages");
          this.dialogRef.close(this.itemForm.value);
      }, error => {
        this.handleError(error, "Error saving meta widget");
      });
    }
  }

  setContentTitle(title: string) {
    let self = this;
    self.viewModel.selectedTitle = title;

    let contentItem = self.viewModel.contentItems.find(ci => ci.title == title);

    self.updateForm(self.itemForm, {itemTitle: contentItem.title});
    self.updateForm(self.itemForm, {itemSubtitle: contentItem.subTitle});
    self.updateForm(self.itemForm, {itemContent: contentItem.content});
    self.updateForm(self.itemForm, {itemContentPageId: +contentItem.contentPage});
  }

  addItem() {
    let self = this;

    const title = '** PLEASE ADD TITLE HERE **';
    let index = self.viewModel.contentItems.findIndex(ci => ci.title == title);
    if(index > -1) {
      self.viewModel.displayDuplicateError = true;
      setTimeout(function() {
        self.viewModel.displayDuplicateError = false;
      }, 3000);
      return;
    }

    const contentItem: WidgetListItemModel = {
      title: title,
      subTitle: '',
      content: '** PLEASE ADD CONTENT HERE **',
      contentPage: '1',
      modified: ''
    };
    self.viewModel.contentItems.push(contentItem);

    self.viewModel.selectedTitle = '';
  }

  removeItem() {
    let self = this;
    let index = self.viewModel.contentItems.findIndex(ci => ci.title == self.viewModel.selectedTitle);
    self.viewModel.contentItems.splice(index, 1)

    self.viewModel.selectedTitle = '';
  }

  updateItem() {
    let self = this;

    let contentItem = self.viewModel.contentItems.find(ci => ci.title == self.viewModel.selectedTitle);
    contentItem.title = self.itemForm.get('itemTitle').value;
    contentItem.subTitle = self.itemForm.get('itemSubtitle').value;
    contentItem.content = self.itemForm.get('itemContent').value;
    contentItem.contentPage = self.itemForm.get('itemContentPageId').value;

    self.viewModel.selectedTitle = '';
  }
}

class ViewModel {
  selectedIcon: string = '';
  selectedTitle: string = '';
  displayDuplicateError = false;
  contentItems: WidgetListItemModel[];
}

export interface WidgetConfigurePopupData {
  metaPageId: number;
  metaWidgetId: number;
  title: string;
}

export const widgetLocationValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  if (!control.parent || !control)
      return null;

  const widgetStatus = control.parent.get('widgetStatus');
  const widgetLocation = control.parent.get('widgetLocation');

  if (!widgetStatus || !widgetLocation)
      return null;

  if (!widgetStatus.value || !widgetLocation.value ||
    widgetStatus.value === '' || widgetLocation.value === '')
      return null;

  if (widgetStatus.value == 'Unpublished')
      return null;

  if (widgetLocation.value != 'Unassigned')
      return null;

  return { 'invalidValue': true };
};