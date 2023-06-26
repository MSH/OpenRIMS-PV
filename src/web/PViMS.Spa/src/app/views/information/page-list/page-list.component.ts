import { Component, OnInit, AfterViewInit, OnDestroy, ViewEncapsulation, ViewChild } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MediaObserver } from '@angular/flex-layout';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { takeUntil, finalize } from 'rxjs/operators';
import { GridModel } from 'app/shared/models/grid.model';
import { MetaPageService } from 'app/shared/services/meta-page.service';
import { PageConfigurePopupComponent } from '../shared/page-configure-popup/page-configure.popup.component';
import { _routes } from 'app/config/routes';

@Component({
  templateUrl: './page-list.component.html',
  styleUrls: ['./page-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class PageListComponent  extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected metaPageService: MetaPageService,
    protected dialog: MatDialog,    
    protected mediaObserver: MediaObserver) 
  {
    super(_router, _location, popupService, accountService, eventService);
  }

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
    });    
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
       null, null, self.mainGridPaginator)
       .subscribe(() => { self.loadGrid(); });
    self.loadGrid();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  } 

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    self.metaPageService.getMetaPages(self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
            self.handleError(error, "Error fetching meta pages");
        });
  }

  openPageConfigurePopup() {
    let self = this;
    let title = 'Add Page';
    let dialogRef: MatDialogRef<any> = self.dialog.open(PageConfigurePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { metaPageId: 0, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(result => {
        if(!result) {
          // If user press cancel
          return;
        }
        self.loadGrid();
      })
  }

  navigateToPage(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.information.view(model.id)]);
  }
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['page-name', 'unique-identifier', 'system', 'visible', 'actions']);
}

class GridRecordModel {
  id: number;
  metaPageGuid: string;
  pageName: string;
  pageDefinition: string;
  metaDefinition: string;
  breadCrumb: string;
  system: string;
  visible: string;
}
