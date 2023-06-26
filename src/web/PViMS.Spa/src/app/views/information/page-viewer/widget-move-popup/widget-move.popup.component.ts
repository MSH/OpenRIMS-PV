import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { MetaPageService } from 'app/shared/services/meta-page.service';
import { MetaPageDetailModel } from 'app/shared/models/meta/meta-page.detail.model';

@Component({
  templateUrl: './widget-move.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class WidgetMovePopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: WidgetMovePopupData,
    public dialogRef: MatDialogRef<WidgetMovePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected metaPageService: MetaPageService,
    protected popupService: PopupService,
    protected accountService: AccountService) 
  { 
    super(_router, _location, popupService, accountService);
  }

  pageList: MetaPageDetailModel[] = [];

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this._formBuilder.group({
      widgetName: [self.data.widgetName || ''],
      destinationMetaPageId: ['', Validators.required],
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

  submit() {
    let self = this;
    self.setBusy(true);

    self.metaPageService.moveMetaWidget(self.data.metaPageId, self.data.metaWidgetId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Meta widget moved successfully", "Meta Pages");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
      this.handleError(error, "Error moving meta widget");
    });
  }
}

export interface WidgetMovePopupData {
  metaPageId: number;
  metaWidgetId: number;
  title: string;
  widgetName: string;
}