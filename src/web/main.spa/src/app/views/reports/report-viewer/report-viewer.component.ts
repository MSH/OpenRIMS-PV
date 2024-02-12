import { Component, OnInit, OnDestroy } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { takeUntil, finalize } from 'rxjs/operators';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { NavigationService } from 'app/shared/services/navigation.service';
import { MetaReportExpandedModel } from 'app/shared/models/meta/meta-report.expanded.model';
import { MetaReportService } from 'app/shared/services/meta-report.service';
import { ConfigService } from 'app/shared/services/config.service';
import { GridModel } from 'app/shared/models/grid.model';

@Component({
  templateUrl: './report-viewer.component.html',
  styles: [`
    .mainButton { display: flex; justify-content: flex-end; button { margin-left: auto; } }
  `],  
  animations: egretAnimations
})
export class ReportViewerComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected navigationService: NavigationService,
    protected eventService: EventService,
    protected metaReportService: MetaReportService,
    protected configService: ConfigService,
    public accountService: AccountService,
    protected mediaObserver: MediaObserver) 
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
  metaReport: MetaReportExpandedModel;

  viewModelForm: FormGroup;
  
  metaDate: string = '';

  ngOnInit() {
    const self = this;
    self.initialiseReport();
  }

  // Force an event to refresh the page if the paramter changes (but not the route)
  initialiseReport(): void {
    // Set default values and re-fetch any data you need.
    const self = this;

    self.id = +self._activatedRoute.snapshot.paramMap.get('id');

    self.viewModelForm = self._formBuilder.group({
    });

    self.loadData();
  }
  
  ngAfterViewInit(): void {
    let self = this;
    self.loadMetaDate();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(ReportViewerComponent.name);
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);
    self.metaReportService.getMetaReportByExpanded(self.id)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        console.log(result);
        self.metaReport = result;
      }, error => {
        self.handleError(error, "Error fetching meta report");
      });
  }

  loadMetaDate(): void {
    let self = this;
    self.configService.getConfigIdentifier(2)
      .subscribe(result => {
        self.metaDate = result.configValue
      });
  }
}

// class ViewModel {
//   mainGrid: GridModel<GridRecordModel> =
//       new GridModel<GridRecordModel>
//           (['facility', 'patient-count', 'patient-serious-count', 'patient-non-serious-count', 'event-percentage', 'actions']);

//   criteriaId: number;
//   searchFrom: Moment;
//   searchTo: Moment;
// }
