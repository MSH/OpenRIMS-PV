import { Component, OnInit, OnDestroy, ViewEncapsulation, ViewChild, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { BaseComponent } from 'app/shared/base/base.component';
import { GridModel } from 'app/shared/models/grid.model';
import { Subscription } from 'rxjs';
import { CohortGroupIdentifierModel } from 'app/shared/models/cohort/cohort-group.identifier.model';
import { ConditionService } from 'app/shared/services/condition.service';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { takeUntil, finalize } from 'rxjs/operators';
import { ReportInstanceService } from 'app/shared/services/report-instance.service';
import { Moment } from 'moment';
import { egretAnimations } from 'app/shared/animations/egret-animations';

// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { ConditionIdentifierModel } from 'app/shared/models/condition/condition.identifier.model';
import { RiskFactorService } from 'app/shared/services/risk-factor.service';
import { RiskFactorDetailModel } from 'app/shared/models/risk-factor/risk-factor.detail.model';
import { AnalysisService } from 'app/shared/services/analysis.service';
import { AnalyserTermIdentifierModel } from 'app/shared/models/analysis/analyser-term.identifier.model';
import { SeriesValueListModel } from 'app/shared/models/dataset/series-value-list.model';
import { MatPaginator } from '@angular/material/paginator';
import { WorkFlowService } from 'app/shared/services/work-flow.service';
import { ProgressStatusEnum, ProgressStatus } from 'app/shared/models/program-status.model';
import { HttpEventType } from '@angular/common/http';
import { ApexAxisChartSeries, ApexChart, ApexDataLabels, ApexFill, ApexLegend, ApexPlotOptions, ApexStroke, ApexTooltip, ApexXAxis, ApexYAxis, ChartComponent } from 'ng-apexcharts';

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
  templateUrl: './active-analyser.component.html',
  animations: egretAnimations
})
export class ActiveAnalyserComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild("chart") chart: ChartComponent;
  public exposedChartOptions: Partial<ChartOptions>;
  public relativeChartOptions: Partial<ChartOptions>;

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected eventService: EventService,
    protected conditionService: ConditionService,
    protected cohortGroupService: CohortGroupService,
    protected reportInstanceService: ReportInstanceService,
    protected riskFactorService: RiskFactorService,
    protected analysisService: AnalysisService,
    protected workFlowService: WorkFlowService,
    protected mediaObserver: MediaObserver,
    public accountService: AccountService) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          //this.setupTable();
      }
    });     
  }

  selectedOption = 1;
  selectedAnalysisTerm = '';
  selectedTermId = 0;
  adjustedRelativeRisk = false;

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;
  viewDatasetModelForm: FormGroup;

  conditionList: ConditionIdentifierModel[] = [];
  cohortList: CohortGroupIdentifierModel[] = [];
  riskFactorList: RiskFactorDetailModel[] = [];
  selectedRiskFactor: RiskFactorDetailModel;

  riskFactors: RiskFactorGridRecordModel[] = [];

  step = 0;

  exposedSeries: SeriesValueListModel[];
  riskSeries: SeriesValueListModel[];

  percentage: number;
  showProgress: boolean;

  @ViewChild('mainGridPaginator') mainGridPaginator: MatPaginator;
    
  setStep(index: number) {
    this.step = index;
  }

  nextStep() {
    this.step++;
  }

  prevStep() {
    this.step--;
  }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.viewModelForm = self._formBuilder.group({
      conditionId: [this.viewModel.conditionId || '0', populationGroupValidator],
      cohortGroupId: [this.viewModel.cohortGroupId || '0', populationGroupValidator],
      searchFrom: [this.viewModel.searchFrom || moment().subtract(1, 'years'), Validators.required],
      searchTo: [this.viewModel.searchTo || moment(), Validators.required],
      riskFactorId: [this.viewModel.riskFactorId],
      riskFactorOptionId: [this.viewModel.riskFactorOptionId]
    });

    self.viewDatasetModelForm = self._formBuilder.group({
      workflowId: ['892F3305-7819-4F18-8A87-11CBA3AEE219'],
      datasetCohortGroupId: [this.viewModel.datasetCohortGroupId || '0'],
    });    
  }

  ngAfterViewInit(): void {
    let self = this;
    self.viewModel.exposedGrid.setupBasic(null, null, null);
    self.viewModel.unexposedGrid.setupBasic(null, null, null);
    self.viewModel.riskGrid.setupBasic(null, null, null);

    self.viewModel.patientContributionGrid.setupAdvance(
      null, null, self.mainGridPaginator)
      .subscribe(() => { self.loadPatientGrid(); });
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  selectAction(action: number): void {
    this.selectedOption = action;
  }

  getClassForSelected(action: number): string[] {
    if(action == this.selectedOption) {
      return ['list-item-active'];
    }
  }

  loadDropDowns(): void {
    let self = this;
    self.getConditionList();
    self.getCohortGroupList();
    self.getRiskFactorList();
  }
  
  getConditionList(): void {
    let self = this;
    self.conditionService.getAllConditions()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.conditionList = result;
        }, error => {
            self.handleError(error, "Error fetching conditions");
        });
  }

  getCohortGroupList(): void {
    let self = this;
    self.cohortGroupService.getAllCohortGroups()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.cohortList = result;
        }, error => {
          self.handleError(error, "Error fetching cohort groups");
        });
  }

  getRiskFactorList(): void {
    let self = this;
    self.riskFactorService.getAllRiskFactors()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.riskFactorList = result;
        }, error => {
          self.handleError(error, "Error fetching risk factors");
        });
  }

  resetCriteria(): void {
    let self = this;
    self.viewModel.resultCount = 0;
    self.selectedTermId = 0;
    self.selectedOption = 1;
    self.viewModel.patientContributionGrid.clearDataSource();
    self.viewModel.exposedGrid.clearDataSource();
    self.viewModel.unexposedGrid.clearDataSource();
    self.viewModel.riskGrid.clearDataSource();
  }

  runAnalysisTerm(): void {
    let self = this;
    self.setBusy(true);
    self.analysisService.getAllAnalysisTermSets(self.viewModelForm.value, self.riskFactors)
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
            self.viewModel.results = result;
            self.viewModel.resultCount = result.length;

            self.notify("Analysis terms retrieved successfully. Please check results tab!", "Results");
            self.selectedOption = 2;
        }, error => {
          self.handleError(error, "Error generating analysis terms");
        });    
  }

  runAnalysisResult(termId: number): void {
    let self = this;
    self.selectedTermId = termId;
    self.setBusy(true);
    self.analysisService.getAnalysisTermSetByDetail(self.viewModelForm.value, termId, self.riskFactors)
        .pipe(takeUntil(self._unsubscribeAll))
        .pipe(finalize(() => self.setBusy(false)))
        .subscribe(result => {
          console.log(result);
          self.viewModel.exposedGrid.updateBasic(result.results);
          self.viewModel.unexposedGrid.updateBasic(result.results);
          self.viewModel.riskGrid.updateBasic(result.results);

          self.exposedSeries = result.exposedCaseSeries;
          self.riskSeries = result.relativeRiskSeries;

          self.loadPatientGrid();
        }, error => {
          self.handleError(error, "Error generating analysis results");
        });    
  }

  loadPatientGrid(): void {
    let self = this;
    self.setBusy(true);
    self.analysisService.getAnalysisPatientSet(self.viewModel.patientContributionGrid.customFilterModel(self.viewModelForm.value), self.selectedTermId, self.riskFactors)
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
          self.viewModel.patientContributionGrid.updateAdvance(result);
      }, error => {
          self.handleError(error, "Error generating patient set");
      });    
  }

  disableAnalysisCriteria(): boolean {
    let self = this;
    return self.selectedTermId > 0;
  }
  
  disableAnalysisResult(): boolean {
    let self = this;
    return self.viewModel.resultCount == 0;
  }

  disableAnalysisOther(): boolean {
    let self = this;
    return self.selectedTermId == 0;
  }

  changeCondition(e) {
    if (e.source.value > 0) {
      let self = this;
      self.viewModel.cohortGroupId = 0;
      self.viewModelForm.patchValue({cohortGroupId: 0});
      self.viewModelForm.updateValueAndValidity({ onlySelf: false, emitEvent: true});
      self.viewModelForm.get('cohortGroupId').updateValueAndValidity({ onlySelf: true, emitEvent: true});
      self.viewModelForm.get('conditionId').updateValueAndValidity({ onlySelf: true, emitEvent: true});
      self.viewModelForm.updateValueAndValidity({ onlySelf: true, emitEvent: true});
     }
  }
  
  changeCohort(e) {
    if (e.source.value > 0) {
      let self = this;
      self.viewModel.conditionId = 0;
      self.viewModelForm.patchValue({conditionId: 0});
      self.viewModelForm.get('cohortGroupId').updateValueAndValidity({ onlySelf: true, emitEvent: true});
      self.viewModelForm.get('conditionId').updateValueAndValidity({ onlySelf: true, emitEvent: true});
      self.viewModelForm.updateValueAndValidity({ onlySelf: true, emitEvent: true});
     }
  }

  changeRiskFactor(e) {
    const value = e.source.value;
    this.selectedRiskFactor = this.riskFactorList.filter(c => c.id == value)[0];
    this.viewModelForm.patchValue({riskFactorOptionId: 0});
  }

  addRiskFactor(data: any = {}, isNew?) {
    let self = this;
    
    // Only one option allowed per risk factor
    if (this.riskFactors.findIndex(c => c.factorName == self.selectedRiskFactor.factorName) > -1) { return; }

    let riskFactor: RiskFactorGridRecordModel = {
      index: isNew ? this.riskFactors.length : data.index,
      factorName: self.selectedRiskFactor.factorName,
      optionName: self.viewModelForm.get('riskFactorOptionId').value,
    };
    if(isNew) {
      this.riskFactors.push(riskFactor);
    }
    else {
      this.riskFactors[data.index] = riskFactor;
    }
    this.viewModel.riskFactorGrid.updateBasic(this.riskFactors);
  }

  removeRiskFactor(index: number): void {
    let self = this;
    let newIndex = self.riskFactors.findIndex(rf => rf.index === index);
    self.riskFactors.splice(newIndex, 1)
    this.viewModel.riskFactorGrid.updateBasic(this.riskFactors);
  }

  downloadDataset(): void {
    let self = this;
    this.downloadStatus( {status: ProgressStatusEnum.START});

    this.workFlowService.downloadDataset(self.viewDatasetModelForm.value).subscribe(
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

            this.notify("Dataset downloaded successfully!", "Success");            
            break;
        }
      },
      error => {
        this.downloadStatus( {status: ProgressStatusEnum.ERROR} );
      }
    );
  }

  downloadStatus(event: ProgressStatus) {
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
}

class ViewModel {
  riskFactorGrid: GridModel<RiskFactorGridRecordModel> =
    new GridModel<RiskFactorGridRecordModel>
        (['factor-name', 'option-name', 'actions']);

  exposedGrid: GridModel<IncidenceRateGridRecordModel> =
    new GridModel<IncidenceRateGridRecordModel>
        (['medication', 'cases', 'non-cases', 'population', 'incidence rate']);

  unexposedGrid: GridModel<IncidenceRateGridRecordModel> =
    new GridModel<IncidenceRateGridRecordModel>
        (['medication', 'cases', 'non-cases', 'population', 'incidence rate']);

  riskGrid: GridModel<RelativeRiskGridRecordModel> =
    new GridModel<RelativeRiskGridRecordModel>
        (['medication', 'unadjusted relative risk', 'adjusted relative risk', 'CI 95%']);

  patientContributionGrid: GridModel<PatientContributionGridRecordModel> =
    new GridModel<PatientContributionGridRecordModel>
        (['patient-name', 'medication', 'days-contributed', 'reaction', 'factor', 'criteria', 'factor-met']);
    
  resultCount: number = 0;
  results: AnalyserTermIdentifierModel[];

  conditionId: number;
  cohortGroupId: number;
  searchFrom: any;
  searchTo: any;
  riskFactorId: number;
  riskFactorOptionId: number;

  termId: number;

  datasetCohortGroupId: number;
}

class IncidenceRateGridRecordModel {
  medication: string;
  cases: number;
  nonCases: number
  population: number;
  incidenceRate: number;
}

class RelativeRiskGridRecordModel {
  medication: string;
  unadjustedRelativeRisk: string;
  adjustedRelativeRisk: string;
  confidenceIntervalLow: string;
  confidenceIntervalHigh: string;
}

class PatientContributionGridRecordModel {
  patientName: string;
  medication: string;
  startDate: string;
  finishDate: string;
  daysContributed: number;
  experiencedReaction: string;
  riskFactor: string;
  riskFactorOption: string;
  factorMet: string;
}

class RiskFactorGridRecordModel {
  index: number;
  factorName: string;
  optionName: string;
}

export const populationGroupValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {

  if (!control.parent || !control)
      return null;

  const conditionId = control.parent.get('conditionId');
  const cohortGroupId = control.parent.get('cohortGroupId');

  if (conditionId.value as number == 0 && cohortGroupId.value as number == 0)
    return { 'populationInvalid': true };
    
  if (conditionId.value as number > 0 && cohortGroupId.value as number > 0)
    return { 'populationInvalid': true };

  return null;
};
