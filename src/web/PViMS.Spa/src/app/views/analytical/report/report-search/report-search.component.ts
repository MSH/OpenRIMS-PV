import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { forkJoin, Subscription } from 'rxjs';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CriteriaListModel } from 'app/shared/models/criteria.list.model';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { Moment } from 'moment';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MedicationListPopupComponent } from './medications-popup/medication-list.popup.component';
import { TerminologyMedDraModel } from 'app/shared/models/terminologymeddra.model';
import { _routes } from 'app/config/routes';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { ActivityStatusChangePopupComponent } from '../activity-status-change-popup/activity-status-change.popup.component';
import { ProgressStatusEnum, ProgressStatus } from 'app/shared/models/program-status.model';
import { HttpEventType } from '@angular/common/http';
import { PatientService } from 'app/shared/services/patient.service';
import { NaranjoPopupComponent } from './naranjo-popup/naranjo.popup.component';
import { ReportInstanceMedicationDetailModel } from 'app/shared/models/report-instance/report-instance-medication.detail.model';
import { SetMeddraPopupComponent } from './set-meddra-popup/set-meddra.popup.component';
import { DatasetInstancePopupComponent } from './dataset-instance-popup/dataset-instance.popup.component';
import { WorkFlowService } from 'app/shared/services/work-flow.service';
import { WorkFlowDetailModel } from 'app/shared/models/work-flow/work-flow.detail.model';
import { LinkModel } from 'app/shared/models/link.model';
import { DatasetInstanceModel } from 'app/shared/models/dataset/dataset-instance-model';
import { WhoPopupComponent } from './who-popup/who.popup.component';
import { SetClassificationPopupComponent } from './set-classification/set-classification.popup.component';
import { ApexAxisChartSeries, ApexChart, ApexDataLabels, ApexFill, ApexLegend, ApexPlotOptions, ApexStroke, ApexTooltip, ApexXAxis, ApexYAxis, ChartComponent } from 'ng-apexcharts';
import { WorkFlowSummaryModel } from 'app/shared/models/work-flow/work-flow.summary.model';
import { SeriesValueListModel } from 'app/shared/models/dataset/series-value-list.model';
import { SeriesValueListItemModel } from 'app/shared/models/dataset/series-value-list-item.model';

const moment =  _moment;

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  fill: ApexFill;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  legend: ApexLegend;
};

@Component({
  templateUrl: './report-search.component.html',
  styles: [`
    .mat-column-id { flex: 0 0 5% !important; width: 5% !important; }
    .mat-column-identifier { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-patient { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-medication-summary { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-actions { flex: 0 0 5% !important; width: 5% !important; }
  `],
  animations: egretAnimations
})
export class ReportSearchComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild("chart") chart: ChartComponent;
  public classificationChartOptions: Partial<ChartOptions>;
  public facilityChartOptions: Partial<ChartOptions>;

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected reportInstanceService: ReportInstanceService,
    protected eventService: EventService,
    protected patientService: PatientService,
    protected workFlowService: WorkFlowService,    
    protected mediaObserver: MediaObserver,
    protected dialog: MatDialog) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          this.setupTable();
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

  percentage: number;
  showProgress: boolean;

  navigationSubscription;
  workflowId: string = null;
  qualifiedName: string = null;

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  criteriaList: CriteriaListModel[] = [];
  workFlow: WorkFlowDetailModel;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;

  ngOnInit(): void {
    const self = this;
    self.initialiseReport();
  }

  // Force an event to refresh the page if the paramter changes (but not the route)
  initialiseReport(): void {
    // Set default values and re-fetch any data you need.
    const self = this;

    // Only execute if user is not logged out
    if(!self.accountService.hasToken())
    {
      return;
    }

    self.workflowId = self._activatedRoute.snapshot.paramMap.get('wuid');
    self.qualifiedName = self._activatedRoute.snapshot.paramMap.get('qualifiedName');

    self.viewModelForm = self._formBuilder.group({
      qualifiedName: [self.viewModel.qualifiedName || ''],
      searchFrom: [self.viewModel.searchFrom || moment().subtract(3, 'months'), Validators.required],
      searchTo: [self.viewModel.searchTo || moment(), Validators.required],
      searchTerm: [self.viewModel.searchTerm || '']
    });

    self.viewModel.mainGrid.clearDataSource();
    self.loadData();
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
      null, null, self.mainGridPaginator)
      .subscribe(() => { self.loadGrid(); });
   
    self.loadData();
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(ReportSearchComponent.name);
  } 

  setupTable() {
    if (this.currentScreenWidth === 'xs') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['status', 'actions']);
    }
    if (this.currentScreenWidth === 'sm') 
    { 
      this.viewModel.mainGrid.updateDisplayedColumns(['patient-identifier', 'status', 'actions']);
    }
  };

  downloadAttachment(data: any = {}): void {
    let self = this;
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.reportInstanceService.downloadAttachment(self.workflowId, data.id, data.activityExecutionStatusEventId, data.attachmentId).subscribe(
      data => {
        switch (data.type) {
          case HttpEventType.DownloadProgress:
            this.downloadStatus( {status: ProgressStatusEnum.IN_PROGRESS, percentage: Math.round((data.loaded / data.total) * 100)});
            break;

          case HttpEventType.Response:
            this.downloadStatus( {status: ProgressStatusEnum.COMPLETE});
            
            const downloadedFile = new Blob([data.body], { type: data.body.type });
            const a = document.createElement('a');

            a.setAttribute('style', 'display:none;');
            document.body.appendChild(a);
            a.download = '';
            a.href = URL.createObjectURL(downloadedFile);
            a.target = '_blank';
            a.click();
            document.body.removeChild(a);
            break;
        }
      },
      error => {
        this.downloadStatus( {status: ProgressStatusEnum.ERROR} );
      }
    );
  }

  downloadSummary(data: any = {}): void {
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.reportInstanceService.downloadSummary(this.workflowId, data.id).subscribe(
      data => {
        switch (data.type) {
          case HttpEventType.DownloadProgress:
            this.downloadStatus( {status: ProgressStatusEnum.IN_PROGRESS, percentage: Math.round((data.loaded / data.total) * 100)});
            break;

          case HttpEventType.Response:
            this.downloadStatus( {status: ProgressStatusEnum.COMPLETE});
            
            const downloadedFile = new Blob([data.body], { type: data.body.type });
            const a = document.createElement('a');

            a.setAttribute('style', 'display:none;');
            document.body.appendChild(a);
            a.download = '';
            a.href = URL.createObjectURL(downloadedFile);
            a.target = '_blank';
            a.click();
            document.body.removeChild(a);
            break;
        }
      },
      error => {
        console.log(error);
        this.downloadStatus( {status: ProgressStatusEnum.ERROR} );
      }
    );
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    requestArray.push(self.workFlowService.getWorkFlowDetail(self.workflowId));
    requestArray.push(self.workFlowService.getWorkFlowSummary(self.workflowId));

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.workFlow = data[0] as WorkFlowDetailModel;
          if(self.qualifiedName != null) {
            self.selectActivity(self.qualifiedName);
          }
  
          self.viewModel.workFlowSummary = data[1] as WorkFlowSummaryModel;

          self.pepareChartSeries();
          self.initClassificationChart();

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        }); 
  }  

  selectActivity(activity: string): void {
    let self = this;

    self.updateForm(self.viewModelForm, {qualifiedName: activity});
    if(activity == 'Summary') {
      return;
    }

    if(activity == "Confirm Report Data") {
      self.viewModel.mainGrid.updateDisplayedColumns(['patient-identifier', 'created', 'adverse-event', 'task-count', 'status', 'actions'])
    }
    else {
      self.viewModel.mainGrid.updateDisplayedColumns(['patient-identifier', 'created', 'medication-summary', 'adverse-event', 'meddra-term', 'status', 'actions'])
    }

    self.viewModel.searchContext = activity == "New reports" ? "New" : "Active";
    self.loadGrid();
  }

  searchByDate(): void {
    let self = this;

    self.viewModel.searchContext = "Date";
    self.loadGrid();
  }

  searchByTerm(): void {
    let self = this;

    self.viewModel.searchContext = "Term";
    self.loadGrid();
  }

  loadGrid(): void {
    let self = this;
    self.setBusy(true);

    switch (self.viewModel.searchContext) {
      case "New":
        self.reportInstanceService.getNewReportInstancesByDetail(self.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error getting new report instances");
        });
  
        break;

      case "Active":
        self.reportInstanceService.getAnalysisReportInstancesByDetail(self.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error getting report instances by activity");
        });
  
        break;

      case "Date":
        self.reportInstanceService.searchReportInstanceByDate(self.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching for report instances by date");
        });
    
        break;

      case "Term":
        self.reportInstanceService.searchReportInstanceByTerm(self.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching for report instances by term");
        });
    
        break;
    }    
  }    

  openMedicationPopUp(medications: ReportInstanceMedicationDetailModel[], data: any = {}) {
    let self = this;
    let title = 'View Medications';

    let dialogRef: MatDialogRef<any> = self.dialog.open(MedicationListPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { medications: medications, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
      })
  }

  detailActivity(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.analytical.reports.activity(self.workflowId, model ? model.id : 0)]);
  }

  detailTask(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.analytical.reports.task(self.workflowId, model ? model.id : 0)]);
  }

  detailPatient(model: GridRecordModel = null): void {
    let self = this;
    self._router.navigate([_routes.clinical.patients.view(model ? model.patientId : 0)]);
  }

  detailClinical(patientId: number, patientClinicalEventId: number) {
    let self = this;
    self._router.navigate([_routes.analytical.reports.clinical(patientId, patientClinicalEventId)]);
  }  

  openActivityChangePopUp(title: string, data: any = {}, newStatus: string) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(ActivityStatusChangePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { workFlowId: this.workflowId, title: title, reportInstanceId: data.id, currentStatus: data.currentStatus, newStatus }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
        self.loadData();
      })
  }

  openSetMeddraPopUp(data: any = {}) {
    let self = this;
    let title = 'Set Terminology';
    let dialogRef: MatDialogRef<any> = self.dialog.open(SetMeddraPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { workFlowId: this.workflowId, title: title, reportInstanceId: data.id }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
        self.loadData();
      })
  }  

  openSetClassificationPopUp(data: any = {}) {
    let self = this;
    let title = 'Set Classification';
    let dialogRef: MatDialogRef<any> = self.dialog.open(SetClassificationPopupComponent, {
      width: '720px',
      disableClose: true,
      data: { workFlowId: this.workflowId, title: title, reportInstanceId: data.id }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
        self.loadData();
      })
  }

  openNaranjoPopUp(data: any = {}) {
    let self = this;
    let title = 'Naranjo Causality';
    let dialogRef: MatDialogRef<any> = self.dialog.open(NaranjoPopupComponent, {
      width: '1000px',
      disableClose: true,
      data: { workFlowId: this.workflowId, title: title, reportInstanceId: data.id }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        self.loadGrid();
        self.loadData();
      })
  }

  openWhoPopUp(data: any = {}) {
    let self = this;
    let title = 'WHO Causality';
    let dialogRef: MatDialogRef<any> = self.dialog.open(WhoPopupComponent, {
      width: '1000px',
      disableClose: true,
      data: { workFlowId: this.workflowId, title: title, reportInstanceId: data.id }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        self.loadGrid();
        self.loadData();
      })
  }

  openDatasetPopUp(title: string, datasetId: number, datasetInstanceId: number) {
    let self = this;
    let dialogRef: MatDialogRef<any> = self.dialog.open(DatasetInstancePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { datasetId: datasetId, title: title, instanceId: datasetInstanceId }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        self.loadGrid();
        self.loadData();
      })
  }

  createE2B(data: any = {}) {
    let self = this;
    self.setBusy(true);

    self.reportInstanceService.createE2B(self.workflowId, data.id)
    .pipe(finalize(() => self.setBusy(false)))
    .subscribe(result => {
      self.notify("E2B extract created successfully", "Activity");
      self.loadGrid();
      self.loadData();
    }, error => {
      this.handleError(error, "Error creating E2B dataset");
    });
  }

  hasLink(data: any = {}, rel: string): boolean {
    return data.links.find(l => l.rel == rel) != null;
  }

  public downloadStatus(event: ProgressStatus) {
    switch (event.status) {
      case ProgressStatusEnum.START:
        this.setBusy(true);
        break;

      case ProgressStatusEnum.IN_PROGRESS:
        this.showProgress = true;
        this.percentage = event.percentage;
        break;

      case ProgressStatusEnum.COMPLETE:
        this.showProgress = false;
        this.setBusy(false);
        break;

      case ProgressStatusEnum.ERROR:
        this.showProgress = false;
        this.setBusy(false);
        this.throwError('Error downloading file. Please try again.', 'Error downloading file. Please try again.');
        break;
    }
  }

  private pepareChartSeries() {
    const self = this;

    const aesi: SeriesValueListModel = 
    {
      name: 'AESI',
      series: self.prepareSeriesItem(self.viewModel.workFlowSummary.classifications.find(c => c.classification == 'AESI'))
    };
    self.viewModel.mainSeries.push(aesi);
    const sae: SeriesValueListModel = 
    {
      name: 'SAE',
      series: self.prepareSeriesItem(self.viewModel.workFlowSummary.classifications.find(c => c.classification == 'SAE'))
    };
    self.viewModel.mainSeries.push(sae);
    const significant: SeriesValueListModel = 
    {
      name: 'Clinically Significant',
      series: self.prepareSeriesItem(self.viewModel.workFlowSummary.classifications.find(c => c.classification == 'Clinically Significant'))
    };
    self.viewModel.mainSeries.push(significant);
  }

  private prepareSeriesItem(classification: any): SeriesValueListItemModel[] {
    const self = this;
    let series: SeriesValueListItemModel[] = [];
    if(classification == null) {
      return series;
    }
    const classificationValueItem: SeriesValueListItemModel = {
      name: 'Classifications',
      value: classification.classificationCount
    };
    series.push(classificationValueItem);
    const causalityValueItem: SeriesValueListItemModel = {
      name: 'One drug causative',
      value: classification.causativeCount
    };
    series.push(causalityValueItem);
    const e2bValueItem: SeriesValueListItemModel = {
      name: 'E2B',
      value: classification.e2BCount
    };
    series.push(e2bValueItem);
    return series;
  }  

  private initClassificationChart() {
    this.classificationChartOptions = {
      series: [
        {
          name: "Submissions",
          data: this.getValueFromSeries(0)
        },
        {
          name: "Least one drug causative",
          data: this.getValueFromSeries(1)
        },
        {
          name: "E2B Submitted",
          data: this.getValueFromSeries(2)
        }
      ],
      chart: {
        type: "bar",
        height: 350
      },
      plotOptions: {
        bar: {
          horizontal: false,
          columnWidth: "45%"
        }
      },
      dataLabels: {
        enabled: true
      },
      stroke: {
        show: true,
        width: 2,
        colors: ["transparent"]
      },
      xaxis: {
        categories: ['SAE', 'AESI', 'Clinically Significant']
      },
      yaxis: {
        title: {
          text: ""
        }
      },
      fill: {
        opacity: 1
      },
      tooltip: {
        y: {
          formatter: function(val) {
            return val + " reports";
          }
        }
      }
    };
  }

  private getValueFromSeries(valueItem: number): any[] {
    let values: number[] = [];
    if(this.viewModel.mainSeries[0]?.series.length > 0) {
      values.push(this.viewModel.mainSeries[0]?.series[valueItem]?.value);
    }
    if(this.viewModel.mainSeries[1]?.series.length > 0) {
      values.push(this.viewModel.mainSeries[1]?.series[valueItem]?.value);
    }
    if(this.viewModel.mainSeries[2]?.series.length > 0) {
      values.push(this.viewModel.mainSeries[2]?.series[valueItem]?.value);
    }
    return values;
  }  
}

class ViewModel {
  mainGrid: GridModel<GridRecordModel> =
      new GridModel<GridRecordModel>
          (['patient-identifier', 'created', 'medication-summary', 'adverse-event', 'meddra-term', 'task-count', 'status', 'actions']);

  searchContext: '' | 'New' | 'Active' | 'Date' | 'Term' = '';

  workFlowSummary: WorkFlowSummaryModel;

  qualifiedName: string;
  searchFrom: Moment;
  searchTo: Moment;
  searchTerm: string;

  mainSeries: SeriesValueListModel[] = [];
}

class GridRecordModel {
  id: number;
  createdDetail: string;
  identifier: string;
  patientIdentifier: string;
  medications: ReportInstanceMedicationDetailModel[];
  sourceIdentifier: string;
  terminologyMedDra?: TerminologyMedDraModel;
  e2BInstance?: DatasetInstanceModel;
  spontaneousInstance?: DatasetInstanceModel;
  patientId: number;
  patientClinicalEventId: number;
  activityExecutionStatusEventId: number;
  attachmentId: number;
  taskCount: number;
  qualifiedName: string;
  currentStatus: string;
  links: LinkModel[];
}