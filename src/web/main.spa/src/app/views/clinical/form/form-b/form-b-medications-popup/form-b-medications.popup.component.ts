import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { ProductIdentifierModel } from 'app/shared/models/concepts/product.identifier.model';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { ConceptService } from 'app/shared/services/concept.service';
import * as moment from 'moment';

@Component({
  templateUrl: './form-b-medications.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormBMedicationsPopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: FormBMedicationsPopupData,
    public dialogRef: MatDialogRef<FormBMedicationsPopupComponent>,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
    protected conceptService: ConceptService
  ) { }

  productList: ProductIdentifierModel[] = [];
  filteredProductList: Observable<ProductIdentifierModel[]>;
  frequencyList: string[] = ['6hrs-14hrs-22hrs', '6hrs-12hrs-18hrs-24hrs', '6hrs-18hrs', 'Single dose', 'Once a day', 'Once a weeK'];

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.itemForm = this.formBuilder.group({
      medication: [this.data.payload.medication || '', Validators.required],
      medicationStartDate: [ Object.keys(this.data.payload).length > 0 ? moment(this.data.payload.startDate, "YYYY-MM-DD") : '', Validators.required],
      medicationEndDate: [ Object.keys(this.data.payload).length > 0 ? this.data.payload.endDate.length > 0 ? moment(this.data.payload.endDate, "YYYY-MM-DD") : '' : '' ],
      dose: [this.data.payload.dose || '', Validators.maxLength(30)],
      frequency: [this.data.payload.frequency || '', Validators.maxLength(30)],
      continued: [this.data.payload.continued || '']
    })

    this.filteredProductList = this.itemForm.controls['medication'].valueChanges
      .pipe(
        startWith(''),
        map(value => value.length >= 3 ? this._productFilter(value) : [])
    );
  }

  loadDropDowns(): void {
    let self = this;
    self.getProductList();
  }

  getProductList(): void {
    let self = this;
    self.conceptService.getAllProducts()
      .subscribe(result => {
          self.productList = result;
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
    self.notify("Medication saved successfully", "Medications");
    this.dialogRef.close(this.itemForm.value);
  }

  private _productFilter(value: string): ProductIdentifierModel[] {
    const filterValue = value.toLowerCase();
    return this.productList.filter(option => option.displayName.toLowerCase().includes(filterValue));
  }
}

export interface FormBMedicationsPopupData {
  title: string;
  payload: any;
}