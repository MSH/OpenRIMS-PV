import { Component, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { forkJoin, Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent } from 'app/shared/base/base.component';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { GridModel } from 'app/shared/models/grid.model';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { Moment } from 'moment';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { TerminologyMedDraModel } from 'app/shared/models/terminologymeddra.model';
import { _routes } from 'app/config/routes';
import { PatientService } from 'app/shared/services/patient.service';
import { WorkFlowService } from 'app/shared/services/work-flow.service';
import { WorkFlowDetailModel } from 'app/shared/models/work-flow/work-flow.detail.model';
import { ClinicalEventTaskPopupComponent } from '../clinical-event-task-popup/clinical-event-task.popup.component';
import { WorkFlowSummaryModel } from 'app/shared/models/work-flow/work-flow.summary.model';
import { SeriesValueListModel } from 'app/shared/models/dataset/series-value-list.model';
import { SeriesValueListItemModel } from 'app/shared/models/dataset/series-value-list-item.model';
import { ApexAxisChartSeries, ApexChart, ApexDataLabels, ApexFill, ApexLegend, ApexPlotOptions, ApexStroke, ApexTooltip, ApexXAxis, ApexYAxis, ChartComponent } from 'ng-apexcharts';
import { ClinicalEventViewPopupComponent } from '../../shared/clinical-event-view-popup/clinical-event-view.popup.component';

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
  templateUrl: './feedback-search.component.html',
  styles: [`
    .mat-column-created { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-patient { flex: 0 0 10% !important; width: 10% !important; }
    .mat-column-meddra-term { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-task-count { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-status { flex: 0 0 15% !important; width: 15% !important; }
    .mat-column-actions { flex: 0 0 10% !important; width: 10% !important; }
  `],
  animations: egretAnimations
})
export class FeedbackSearchComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {
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
          //this.setupTable();
      }
    });
  }

  navigationSubscription;

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;
  
  ngOnInit(): void {
    const self = this;

    self.viewModelForm = self._formBuilder.group({
      qualifiedName: [self.viewModel.qualifiedName || ''],
      searchTerm: [self.viewModel.searchTerm || ''],
    });    
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.mainGrid.setupAdvance(
      null, null, self.mainGridPaginator)
      .subscribe(() => { self.loadGrid(); });
  
    self.loadData();
    self.loadGrid();
  }  

  loadData(): void {
    let self = this;
    self.setBusy(true);

    const requestArray = [];

    requestArray.push(self.workFlowService.getWorkFlowDetail(self.viewModel.workflowId));
    requestArray.push(self.workFlowService.getWorkFlowSummary(self.viewModel.workflowId));

    forkJoin(requestArray)
      .subscribe(
        data => {
          self.viewModel.workFlow = data[0] as WorkFlowDetailModel;
          self.viewModel.workFlowSummary = data[1] as WorkFlowSummaryModel;

          self.pepareChartSeries();
          self.initClassificationChart();

          self.setBusy(false);
        },
        error => {
          this.handleError(error, "Error preparing view");
        }); 
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FeedbackSearchComponent.name);
  }

  selectActivity(activity: string): void {
    let self = this;

    self.updateForm(self.viewModelForm, {qualifiedName: activity});
    if(activity == 'Summary') {
      return;
    }

    switch(activity) { 
      case 'Confirm Report Data': { 
        self.viewModel.mainGrid.updateDisplayedColumns(['created', 'patient', 'adverse-event', 'task-count', 'status', 'actions'])
        break; 
      } 
      case 'Set MedDRA and Causality': { 
        self.viewModel.mainGrid.updateDisplayedColumns(['created', 'patient', 'adverse-event', 'meddra-term', 'status', 'actions'])
         break; 
      } 
      default: { 
         //statements; 
         break; 
      } 
    } 

    self.viewModel.searchContext = "Activity";
    self.loadGrid();
  }

  hasActivity(): boolean {
    let self = this;

    let index = self.viewModel?.workFlow?.feedbackActivity.findIndex(fa => fa.reportInstanceCount > 0);
    return index > -1;
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
      case "Activity":
        self.reportInstanceService.getFeedbackReportInstancesByDetail(self.viewModel.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error getting report instances by activity");
        });
  
        break;

      case "Term":
        self.reportInstanceService.searchReportInstanceByTerm(self.viewModel.workflowId, self.viewModel.mainGrid.customFilterModel(self.viewModelForm.value))
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          self.viewModel.mainGrid.updateAdvance(result);
        }, error => {
          this.handleError(error, "Error searching for report instances by term");
        });
    
        break;
    
        break;
    }    
  }

  openClinicalEventTaskPopUp(data: any = {}) {
    let self = this;
    let title = 'View Adverse Event';
    if(data.qualifiedName == 'Confirm Report Data') {
      let dialogRef: MatDialogRef<any> = self.dialog.open(ClinicalEventTaskPopupComponent, {
        width: '920px',
        disableClose: true,
        data: { patientId: data.patientId, clinicalEventId: data.patientClinicalEventId, reportInstanceId: data.id, title: title }
      })
      dialogRef.afterClosed()
        .subscribe(res => {
          self.selectActivity('Confirm Report Data');
          return;
        })
    }
    else {
      let dialogRef: MatDialogRef<any> = self.dialog.open(ClinicalEventViewPopupComponent, {
        width: '920px',
        disableClose: true,
        data: { patientId: data.patientId, clinicalEventId: data.patientClinicalEventId, title: title }
      })
      dialogRef.afterClosed()
        .subscribe(res => {
          return;
        })
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
          (['created', 'patient', 'adverse-event', 'meddra-term', 'actions']);

  workflowId = '892F3305-7819-4F18-8A87-11CBA3AEE219';
  workFlow: WorkFlowDetailModel;

  workFlowSummary: WorkFlowSummaryModel;

  selectedTab = 0;
  searchContext: '' | 'Activity' | 'Term' = '';

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
  sourceIdentifier: string;
  terminologyMedDra?: TerminologyMedDraModel;
  patientId: number;
  attachmentId: number;
  taskCount: number;
  qualifiedName: string;
  currentStatus: string;
}