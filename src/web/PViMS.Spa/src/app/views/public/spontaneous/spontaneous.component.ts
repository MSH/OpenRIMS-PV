import { Component, OnInit,  OnDestroy, ViewEncapsulation } from '@angular/core';
import { DatePipe, Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, FormControl, FormArray } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { _routes } from 'app/config/routes';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { DatasetService } from 'app/shared/services/dataset.service';
import { finalize } from 'rxjs/operators';
import { DatasetCategoryViewModel } from 'app/shared/models/dataset/dataset-category-view.model';
import { DatasetElementSubViewModel } from 'app/shared/models/dataset/dataset-element-sub-view.model';
import { SpontaneousTablePopupComponent } from './spontaneous-table/spontaneous-table.popup.component';

const moment =  _moment;

@Component({
  templateUrl: './spontaneous.component.html',
  styleUrls: ['./spontaneous.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class SpontaneousComponent extends BaseComponent implements OnInit, OnDestroy {

  public datasetId = 1;
  public datasetCategories: DatasetCategoryViewModel[] = [];
  
  public viewModelForm: FormGroup;
  public formArray: FormArray;

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected datasetService: DatasetService,
    protected dialog: MatDialog,
    protected mediaObserver: MediaObserver,
    protected datePipe: DatePipe) 
  {
    super(_router, _location, popupService, accountService, eventService);

    this.flexMediaWatcher = mediaObserver.media$.subscribe((change: MediaChange) => {
      if (change.mqAlias !== this.currentScreenWidth) {
          this.currentScreenWidth = change.mqAlias;
          //this.setupTable();
      }
    });    
  }

  currentScreenWidth: string = '';
  flexMediaWatcher: Subscription;

  ngOnInit(): void {
    const self = this;

    self.viewModelForm = this._formBuilder.group({
      formArray: this._formBuilder.array([])
    })

    self.loadDataset();
  }

  ngAfterViewInit(): void {
    let self = this;
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(SpontaneousComponent.name);
  }

  loadDataset(): void {
    let self = this;
    self.setBusy(true);
    self.datasetService.getSpontaneousDataset()
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.datasetCategories = result.datasetCategories;
        self.datasetId = result.id;
        
        self.prepareFormArray();
      }, error => {
        self.handleError(error, error.statusText);
      });
  }

  generateColumnArray(elementSubs: DatasetElementSubViewModel[]) : string[] {
    //console.log('display columns');
    let displayColumns: string[] = [];
    if (elementSubs.length > 5) {
      displayColumns = elementSubs.slice(0, 5).map(a => a.datasetElementSubName);
      displayColumns.push('actions');
    }
    else {
      displayColumns = elementSubs.map(a => a.datasetElementSubName);
      displayColumns.push('actions');
    }
    return displayColumns;
  }

  openPopup(arrayIndex: number, rowIndex: number, datasetElementId: number, datasetElementSubs: DatasetElementSubViewModel[], data: any = {}, isNew?) {
    let self = this;
    let title = isNew ? 'Add Record' : 'Update Record';
    let dialogRef: MatDialogRef<any> = self.dialog.open(SpontaneousTablePopupComponent, {
      width: '920px',
      disableClose: true,
      data: { title: title, datasetElementSubs, payload: data }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        if(!res) {
          // If user press cancel
          return;
        }
        // Get existing value for the table element
        let tableValue = self.getTableValueFromArray(arrayIndex);

        // Prepare existing array of table values
        let tableRowsArray: any[] = [];
        if (Object.values(tableValue)[0] != null) {
          tableRowsArray = Object.assign([], Object.values(tableValue)[0]);
        }

        if(isNew) {
          tableRowsArray.push(res.elements);
        }
        else {
          tableRowsArray[rowIndex] = res.elements;
        }
        self.setTableValueArray(arrayIndex, datasetElementId, tableRowsArray);
      })
  }

  removeRecord(arrayIndex: number, rowIndex: number, datasetElementId: number): void {
    let self = this;
    
    // Get existing value for the table element
    let tableValue = self.getTableValueFromArray(arrayIndex);

    // Prepare existing array of table values
    let tableRowsArray: any[] = [];
    if (Object.values(tableValue)[0] != null) {
      tableRowsArray = Object.assign([], Object.values(tableValue)[0]);
    }

    tableRowsArray.splice(rowIndex, 1)
    self.setTableValueArray(arrayIndex, datasetElementId, tableRowsArray);
  }

  saveForm(): void {
    let self = this;
    self.setBusy(true);

    let allModels:any[] = []; 

    const arrayControl = <FormArray>this.viewModelForm.controls["formArray"];
    arrayControl.controls.forEach(formGroup => {
      allModels.push(formGroup.value);
    });

    self.datasetService.saveSpontaneousInstance(self.datasetId, allModels)
      .subscribe(result => {
        self.notify("Report created successfully", "Spontaneous Report");
        self._router.navigate([_routes.security.landing]);
      }, error => {
        self.handleError(error, "Error saving spontaneous report");
      });
  }

  getTableDataSource(arrayIndex: number) : any[] {
    let self = this;
    let tableValue = self.getTableValueFromArray(arrayIndex);

    // Prepare existing array of table values
    let tableRowsArray: any[] = [];
    if (Object.values(tableValue)[0] != null) {
      tableRowsArray = Object.assign([], Object.values(tableValue)[0]);
    }

    return tableRowsArray;
  }

  formatOutput(outputValue: string): string {
    if (moment.isMoment(outputValue)) {
      return this.datePipe.transform(outputValue, 'yyyy-MM-dd')
    }
    return outputValue;
  }

  private prepareFormArray(): void {
    let self = this;
    self.datasetCategories.forEach(category => {
      // add form group per category
      let newGroup = self.addGroupForCategory();
      let elements = newGroup.get('elements') as FormGroup;

      category.datasetElements.forEach(element => {
        // Add elements to form group
        let validators = [ ];
        if(element.required) {
           validators.push(Validators.required);
        }
        if(element.stringMaxLength != null) {
           validators.push(Validators.maxLength(element.stringMaxLength));
        }
        if(element.numericMinValue != null && element.numericMaxValue != null) {
           validators.push(Validators.max(element.numericMaxValue));
           validators.push(Validators.min(element.numericMinValue));
        }

        elements.addControl(element.datasetElementId.toString(), new FormControl(null, validators));
      })
    })
  }

  private getTableValueFromArray(index: number): any {
    let self = this;

    const arrayControl = <FormArray>self.viewModelForm.controls["formArray"];
    let formGroup = arrayControl.controls[index] as FormGroup;
    let elements = formGroup.get('elements') as FormGroup;
    
    return elements.value;
  }

  private setTableValueArray(index: number, datasetElementId: number, value: any[]): void {
    let self = this;

    const arrayControl = <FormArray>self.viewModelForm.controls["formArray"];
    let formGroup = arrayControl.controls[index] as FormGroup;
    let elements = formGroup.get('elements') as FormGroup;
    console.log(elements);
    let control = elements.get(datasetElementId.toString()) as FormControl;
    if(control) {
      control.setValue(value);
    }
  }

  private addGroupForCategory(): FormGroup {
    const arrayControl = <FormArray>this.viewModelForm.controls["formArray"];
    let newGroup = this._formBuilder.group({
      elements: this._formBuilder.group([]),
    });
    arrayControl.push(newGroup);
    return newGroup;
  }
}