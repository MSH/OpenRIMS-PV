import { Component, OnInit, ViewEncapsulation, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { Location } from '@angular/common';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { NavigationService } from 'app/shared/services/navigation.service';
import { EventService } from 'app/shared/services/event.service';
import { MetaPageService } from 'app/shared/services/meta-page.service';
import { AccountService } from 'app/shared/services/account.service';
import { MediaObserver } from '@angular/flex-layout';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { WidgetConfigurePopupComponent } from '../widget-configure-popup/widget-configure.popup.component';
import { MetaWidgetDetailModel } from 'app/shared/models/meta/meta-widget.detail.model';
import { GenericDeletePopupComponent } from '../../shared/generic-delete-popup/generic-delete.popup.component';

@Component({
  selector: 'app-general-widget',
  templateUrl: './general-widget.component.html',
  styleUrls: ['./general-widget.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class GeneralWidgetComponent extends BaseComponent implements OnInit, OnDestroy {
  
  constructor(protected _activatedRoute: ActivatedRoute,
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
  }

  @Input() widget: MetaWidgetDetailModel;
  @Input() pageId: number;
  @Output() initialiseReport = new EventEmitter()

  ngOnInit() {
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(GeneralWidgetComponent.name);
  }

  openDeleteWidgetPopUp(id: number, widgetName: string) {
    let self = this;
    let title = 'Delete Widget';
    let dialogRef: MatDialogRef<any> = self.dialog.open(GenericDeletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { parentId: self.pageId, id: id, type: 'MetaWidget', title: title, name: widgetName }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.initialiseReport.emit();
      })
  }

  openWidgetConfigurePopup(id: number) {
    let self = this;
    let title = id == 0 ? 'Add Widget' : 'Update Widget';
    let dialogRef: MatDialogRef<any> = self.dialog.open(WidgetConfigurePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { metaPageId: self.pageId, metaWidgetId: id, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        self.initialiseReport.emit();
      })
  }
}
