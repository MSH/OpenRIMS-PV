import { Component, OnInit, Inject, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { PopupService } from 'app/shared/services/popup.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { CareEventService } from 'app/shared/services/care-event.service';

@Component({
  templateUrl: './care-event-delete.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class CareEventDeletePopupComponent implements OnInit {
  
  public itemForm: FormGroup;
  protected busy: boolean = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: CareEventPopupData,
    public dialogRef: MatDialogRef<CareEventDeletePopupComponent>,
    protected careEventService: CareEventService,
    protected popupService: PopupService,
    protected formBuilder: FormBuilder,
  ) { }

  ngOnInit(): void {
    const self = this;

    self.itemForm = this.formBuilder.group({
      careEventName: [this.data.payload.careEventName],
    })
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

    self.careEventService.deleteCareEvent(self.data.careEventId)
      .pipe(finalize(() => self.setBusy(false)))
      .subscribe(result => {
        self.notify("Care event deleted successfully", "Care Events");
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

export interface CareEventPopupData {
  careEventId: number;
  title: string;
  payload: any;
}