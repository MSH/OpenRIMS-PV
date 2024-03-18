import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { MetaFormDetailModel } from 'app/shared/models/meta/meta-form.detail.model';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { finalize, map, takeUntil } from 'rxjs/operators';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { PopupService } from 'app/shared/services/popup.service';
import { _routes } from 'app/config/routes';
import { CohortGroupService } from 'app/shared/services/cohort-group.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { CohortGroupDetailModel } from 'app/shared/models/cohort/cohort-group.detail.model';

@Component({
  templateUrl: './form-select.component.html',
  animations: egretAnimations
})
export class FormSelectComponent extends BaseComponent implements OnInit, AfterViewInit {

  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected cohortGroupService: CohortGroupService,
    protected metaFormService: MetaFormService,
  ) 
  { 
    super(_router, _location, popupService, accountService, eventService);    
  }

  ngOnInit(): void {
    const self = this;
    
    self.accountService.connected$.subscribe(val => {
      self.viewModel.connected = val;
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    self.loadCohorts();
  } 

  loadCohorts(): void {
    let self = this;
    self.setBusy(true);

    self.cohortGroupService.getAllCohortGroups()
      .pipe(takeUntil(self._unsubscribeAll))
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.CLog(result, 'result');
        self.viewModel.cohortList = result;
      }, error => {
        self.handleError(error, "Error fetching cohorts");
      });
  } 

  selectCohort(cohort: CohortGroupDetailModel): void {
    let self = this;
    self.viewModel.selectedCohort = cohort;
    self.getMetaFormList(cohort.id);
  }  

  getMetaFormList(cohortId: number): void {
    let self = this;
    self.metaFormService.getAllMetaForms()
    .pipe(
      map((result: MetaFormDetailModel[]) => {
        self.CLog(result, 'result of metaform');
        result.forEach(function (value) {
          self.metaFormService.getAllFormsForType(value.actionName).then(result => {
            self.CLog(result, 'result of get all forms');
            value.unsynchedCount = result.value.filter(v => v.synchStatus == 'Not Synched').length;
            value.completedCount = result.value.filter(v => v.synchStatus == 'Not Synched' && v.completeStatus == 'Complete').length;
            value.synchedCount = result.value.filter(v => v.synchStatus == 'Synched').length;
          }, error => {
            self.throwError(error, error.statusText);
          });              
        })

        return result;
      })
    )    
    .pipe(takeUntil(self._unsubscribeAll))
    .subscribe(result => {
        self.CLog(cohortId, 'cohortId');
        let forms = result as MetaFormDetailModel[];
        self.viewModel.formList = forms;
    }, error => {
        self.throwError(error, error.statusText);
    });
  }

  addForm(metaFormId: number): void {
    let self = this;
    self._router.navigate([_routes.clinical.forms.viewFormDynamic(metaFormId, 0)]);
  }
  
  listForm(selectedOption: string): void {
    let self = this;
    switch (selectedOption) {
      case 'FormA':
        self._router.navigate([_routes.clinical.forms.listForm('FormA')]);
        break;

      case 'FormB':
        self._router.navigate([_routes.clinical.forms.listForm('FormB')]);
        break;

      case 'FormC':
        self._router.navigate([_routes.clinical.forms.listForm('FormC')]);
        break;

      case 'ADR Form':
        self._router.navigate([_routes.clinical.forms.listForm('FormADR')]);
        break;

      case 'Px Form':
        self._router.navigate([_routes.clinical.forms.listForm('FormPx')]);
        break;

      case 'FormATPT':
        self._router.navigate([_routes.clinical.forms.listForm('FormATPT')]);
        break;

      case 'FormBTPT':
        self._router.navigate([_routes.clinical.forms.listForm('FormBTPT')]);
        break;        
  
      case 'FormADTG':
        self._router.navigate([_routes.clinical.forms.listForm('FormADTG')]);
        break;

      case 'FormBDTG':
        self._router.navigate([_routes.clinical.forms.listForm('FormBDTG')]);
        break;
    }

  }

  synchroniseForm(selectedOption: string): void {
    let self = this;
    switch (selectedOption) {
      case 'FormA':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormA')]);
        break;

      case 'FormB':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormB')]);
        break;

      case 'FormC':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormC')]);
        break;

      case 'ADR Form':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormADR')]);
        break;

      case 'Px Form':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormPx')]);
        break;

      case 'FormATPT':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormATPT')]);
        break;

      case 'FormBTPT':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormBTPT')]);
        break;        
  
      case 'FormADTG':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormADTG')]);
        break;

      case 'FormBDTG':
        self._router.navigate([_routes.clinical.forms.synchroniseForm('FormBDTG')]);
        break;
    }
  }
  
  navigateToCohortSelect(): void {
    let self = this;
    //self._router.navigate([_routes.clinical.forms.cohortselect]);
  }
}

class ViewModel {
  cohortGroupId: number;

  cohortName: string;
  cohortCode: string;
  conditionName: string;
    
  cohortList: CohortGroupDetailModel[] = [];
  selectedCohort: CohortGroupDetailModel;

  formList: MetaFormDetailModel[] = [];
  connected: boolean = true;
}