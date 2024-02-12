import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { EncounterTypeService } from 'app/shared/services/encounter-type.service';
import { WorkPlanService } from 'app/shared/services/work-plan.service';
import { WorkPlanIdentifierModel } from 'app/shared/models/work/work-plan.identifier.model';

@Component({
  templateUrl: './encounter-type.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class EncounterTypePopupComponent implements OnInit, AfterViewInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  workPlanList: WorkPlanIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EncounterTypePopupData,
    public dialogRef: MatDialogRef<EncounterTypePopupComponent>,
    protected encounterTypeService: EncounterTypeService,
    protected workPlanService: WorkPlanService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this.formBuilder.group({
      encounterTypeName: [this.data.payload.encounterTypeName || '', [Validators.required, Validators.maxLength(50), Validators.pattern('[a-zA-Z ]*')]],
      workPlanName: [this.data.payload.workPlanName, Validators.required],
      help: [this.data.payload.help || '', [Validators.maxLength(250), Validators.pattern('[a-zA-Z0-9. ]*')]]
    })
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.data.encounterTypeId > 0) {
        //self.loadData();
    }
  }

  loadDropDowns(): void {
    let self = this;
    self.getWorkPlanList();
  }

  getWorkPlanList(): void {
    let self = this;
    self.workPlanService.getWorkPlanList()
        .subscribe(result => {
            self.workPlanList = result.value;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  public setBusy(value: boolean): void {
    setTimeout(() => { this.busy = value; });
  }

  public isBusy(): boolean {
    return this.busy;
  }

  protected notify(message: string, action: string) {
    return this.popupService.notify(message, action);
  }

  protected showError(errorMessage: any, title: string = "Error") {
    this.popupService.showErrorMessage(errorMessage, title);
  }

  protected showInfo(message: string, title: string = "Info") {
    this.popupService.showInfoMessage(message, title);
  }

  protected updateForm(form: FormGroup, value: any): void {
    form.patchValue(value);
  }  

  protected throwError(errorObject: any, title: string = "Exception") {
    if (errorObject.status == 401) {
        this.showError(errorObject.error.message, errorObject.error.statusCodeType);
    } else {
        this.showError(errorObject.message, title);
    }
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.encounterTypeService.saveEncounterType(self.data.encounterTypeId, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Encounter type saved successfully", "Encounter Types");
        this.dialogRef.close(this.itemForm.value);
    }, error => {
        if(error.status == 400) {
          self.showInfo(error.error.message[0], error.statusText);
        } else {
          self.throwError(error, error.statusText);
        }
    });
  }
}

export interface EncounterTypePopupData {
  encounterTypeId: number;
  title: string;
  payload: any;
}