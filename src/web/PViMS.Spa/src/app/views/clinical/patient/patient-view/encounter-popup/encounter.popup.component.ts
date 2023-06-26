import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { AccountService } from 'app/shared/services/account.service';
import { Router } from '@angular/router';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { EncounterService } from 'app/shared/services/encounter.service';
import { EncounterTypeIdentifierModel } from 'app/shared/models/encounter/encounter-type.identifier.model';
import { PriorityIdentifierModel } from 'app/shared/models/encounter/priority.identifier.model';
import { EncounterTypeService } from 'app/shared/services/encounter-type.service';
import { PriorityService } from 'app/shared/services/priority.service';

@Component({
  templateUrl: './encounter.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class EncounterPopupComponent extends BasePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  encounterTypeList: EncounterTypeIdentifierModel[] = [];
  priorityList: PriorityIdentifierModel[] = [];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EncounterPopupData,
    public dialogRef: MatDialogRef<EncounterPopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected encounterService: EncounterService,
    protected encounterTypeService: EncounterTypeService,
    protected priorityService: PriorityService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    this.loadDropDowns();

    this.itemForm = this._formBuilder.group({
        encounterDate: [this.data.payload.encounterDate || '', Validators.required],
        encounterTypeId: ['', Validators.required],
        priorityId: ['', Validators.required],
        notes: [this.data.payload.notes || '', [Validators.maxLength(500), Validators.pattern("[a-zA-Z0-9 '.]*")]]
    })
  }

  loadDropDowns(): void {
    let self = this;
    self.getEncounterTypeList();
    self.getPriorityList();
  }  

  getEncounterTypeList(): void {
    let self = this;

    self.encounterTypeService.getAllEncounterTypes()
        .subscribe(result => {
          self.encounterTypeList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  getPriorityList(): void {
    let self = this;

    self.priorityService.getAllPriorities()
        .subscribe(result => {
          self.priorityList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  submit() {
    let self = this;
    self.setBusy(true);

    self.encounterService.saveEncounter(this.data.patientId, 0, self.itemForm.value)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Encounter successfully saved!", "Success");
        this.dialogRef.close(this.itemForm.value);
      }, error => {
        if(error.status == 400) {
          self.showInfo(error.message[0], error.statusText);
        } else {
          if(Array.isArray(error.message)) {
            self.showInfo(error.message[0], error.statusText);
          }
          else {
            self.throwError(error, error.statusText);
          }
        }
    });
  }
}

export interface EncounterPopupData {
  patientId: number;
  title: string;
  payload: any;
}