import { Component, OnInit, Inject, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { finalize } from 'rxjs/operators';
import { MetaPageExpandedModel } from 'app/shared/models/meta/meta-page.expanded.model';
import { MetaWidgetDetailModel } from 'app/shared/models/meta/meta-widget.detail.model';
import { MetaPageService } from 'app/shared/services/meta-page.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './page-viewer.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class PageViewerPopupComponent extends BasePopupComponent implements OnInit, AfterViewInit {

  public itemForm: FormGroup;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PageViewerPopupData,
    public dialogRef: MatDialogRef<PageViewerPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected metaPageService: MetaPageService,
    protected accountService: AccountService) 
  { 
    super(_router, _location, popupService, accountService);
  }

  metaPage: MetaPageExpandedModel;

  topLeft: MetaWidgetDetailModel;
  topRight: MetaWidgetDetailModel;
  middleLeft: MetaWidgetDetailModel;
  middleRight: MetaWidgetDetailModel;
  bottomLeft: MetaWidgetDetailModel;
  bottomRight: MetaWidgetDetailModel;

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.metaPageId > 0) {
        self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaPageService.getMetaPage(self.data.metaPageId, 'expanded')
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.metaPage = result;

        self.topLeft = self.metaPage.widgets.find(w => w.widgetLocation == 'TopLeft');
        self.topRight = self.metaPage.widgets.find(w => w.widgetLocation == 'TopRight');
        self.middleLeft = self.metaPage.widgets.find(w => w.widgetLocation == 'MiddleLeft');
        self.middleRight = self.metaPage.widgets.find(w => w.widgetLocation == 'MiddleRight');
        self.bottomLeft = self.metaPage.widgets.find(w => w.widgetLocation == 'BottomLeft');
        self.bottomRight = self.metaPage.widgets.find(w => w.widgetLocation == 'BottomRight');

      }, error => {
        self.throwError(error, error.statusText);
      });
  }

  selectTerm(data: any) {
    this.dialogRef.close(data);    
  }
}

export interface PageViewerPopupData {
  metaPageId: number;
  title: string;
}