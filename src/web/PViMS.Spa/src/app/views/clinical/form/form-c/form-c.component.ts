import { Component, OnInit, AfterViewInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { Subscription } from 'rxjs';
import { FacilityService } from 'app/shared/services/facility.service';
import { Moment } from 'moment';
// Depending on whether rollup is used, moment needs to be imported differently.
// Since Moment.js doesn't have a default export, we normally need to import using the `* as`
// syntax. However, rollup creates a synthetic default module and we thus need to import it using
// the `default as` syntax.
import * as _moment from 'moment';
import { LabTestService } from 'app/shared/services/lab-test.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { _routes } from 'app/config/routes';
import { MeddraTermService } from 'app/shared/services/meddra-term.service';
import { Form } from 'app/shared/indexed-db/appdb';
import { ConceptService } from 'app/shared/services/concept.service';
import { FacilityIdentifierModel } from 'app/shared/models/facility/facility.identifier.model';
import { takeUntil } from 'rxjs/operators';
import { FormCompletePopupComponent } from '../form-complete-popup/form-complete.popup.component';
import { MatDialogRef, MatDialog } from '@angular/material/dialog';
import { FormAttachmentModel } from 'app/shared/models/form/form-attachment.model';
import { FormGuidelinesPopupComponent } from '../form-guidelines-popup/form-guidelines.popup.component';

const moment =  _moment;

@Component({
  selector: 'app-form-a',
  templateUrl: './form-c.component.html',
  styleUrls: ['./form-c.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class FormCComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected accountService: AccountService,
    protected eventService: EventService,
    protected facilityService: FacilityService,
    protected labTestService: LabTestService,
    protected conceptService: ConceptService,
    protected meddraTermService: MeddraTermService,
    protected metaFormService: MetaFormService,
    protected dialog: MatDialog,
    protected mediaObserver: MediaObserver) 
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

  isComplete = false;
  isSynched = false;  

  id: number;
  identifier: string;
  viewModel: ViewModel = new ViewModel();
  viewModelForm: FormGroup;
  viewPatientModelForm: FormGroup;
  viewObstetricModelForm: FormGroup;
  viewBirthResultModelForm: FormGroup;
  viewSurfaceExamModelForm: FormGroup;

  facilityList: FacilityIdentifierModel[] = [];

  ngOnInit(): void {
    const self = this;
    self.loadDropDowns();

    self.id = +self._activatedRoute.snapshot.paramMap.get('id');

    self.viewModelForm = self._formBuilder.group({
      formCompleted: [this.viewModel.formCompleted]
    });

    self.viewPatientModelForm = self._formBuilder.group({
      treatmentSiteId: [this.viewModel.treatmentSiteId, Validators.required],
      asmNumber: [this.viewModel.asmNumber, [Validators.required, Validators.maxLength(50)]],
      patientFirstName: [this.viewModel.patientFirstName],
      patientLastName: [this.viewModel.patientLastName],
    });

    self.viewObstetricModelForm = self._formBuilder.group({
      firstPregnancy: [this.viewModel.firstPregnancy],
      previousChildrenAbnormality: [this.viewModel.previousChildrenAbnormality],
      previousChildrenAbnormalityDescription: [this.viewModel.previousChildrenAbnormalityDescription],
      immediateFamilyAbnormality: [this.viewModel.immediateFamilyAbnormality],
      immediateFamilyAbnormalityWho: [this.viewModel.immediateFamilyAbnormalityWho],
      immediateFamilyAbnormalityDescription: [this.viewModel.immediateFamilyAbnormalityDescription],
    });

    self.viewBirthResultModelForm = self._formBuilder.group({
      deliveryDate: [this.viewModel.deliveryDate || moment(), Validators.required],
      placeDelivery: [this.viewModel.placeDelivery],
      placeDeliverySpecify: [this.viewModel.placeDeliverySpecify],
      typeDelivery: [this.viewModel.typeDelivery],
      gestAgeUntilDeliveryWeeks: [this.viewModel.gestAgeUntilDeliveryWeeks, [Validators.max(44), Validators.min(4)]],
      gestAgeUntilDeliveryDays: [this.viewModel.gestAgeUntilDeliveryDays, [Validators.max(308), Validators.min(0)]],
      methodEvaluatePregnancy: [this.viewModel.methodEvaluatePregnancy],
      outcomePregnancy: [this.viewModel.outcomePregnancy],
      numberBabies: [this.viewModel.numberBabies],
      numberBabiesOther: [this.viewModel.numberBabiesOther],
      whoAttendedBirth: [this.viewModel.whoAttendedBirth],
      whoAttendedSpecify: [this.viewModel.whoAttendedSpecify],
      deliveryInduced: [this.viewModel.deliveryInduced],
      howBabyBorn: [this.viewModel.howBabyBorn],
      howBabyBornSpecify: [this.viewModel.howBabyBornSpecify],
      complication: [this.viewModel.complication],
      complicationSpecify: [this.viewModel.complicationSpecify]
    });

    self.viewSurfaceExamModelForm = self._formBuilder.group({
      babyName: [this.viewModel.babyName],
      babyASMNumber: [this.viewModel.babyASMNumber],
      babyAgeDays: [this.viewModel.babyAgeDays, [Validators.max(99), Validators.min(0)]],
      babyGender: [this.viewModel.babyGender],
      babyLength: [this.viewModel.babyLength, [Validators.max(65), Validators.min(0)]],
      headCircumference: [this.viewModel.headCircumference, [Validators.max(44), Validators.min(0)]],
      birthWeight: [this.viewModel.birthWeight, [Validators.max(8000), Validators.min(0)]],
      headInspectionTest: [this.viewModel.headInspectionTest],
      headInspectionObservation: [this.viewModel.headInspectionObservation],
      headInspectionOther: [this.viewModel.headInspectionOther],
      faceExaminationTest: [this.viewModel.faceExaminationTest],
      faceExaminationObservation: [this.viewModel.faceExaminationObservation],
      faceExaminationOther: [this.viewModel.faceExaminationOther],
      eyeExaminationTest: [this.viewModel.eyeExaminationTest],
      eyeExaminationObservation: [this.viewModel.eyeExaminationObservation],
      eyeExaminationOther: [this.viewModel.eyeExaminationOther],
      mouthExaminationTest: [this.viewModel.mouthExaminationTest],
      mouthExaminationObservation: [this.viewModel.mouthExaminationObservation],
      mouthExaminationOther: [this.viewModel.mouthExaminationOther],
      noseExaminationTest: [this.viewModel.noseExaminationTest],
      noseExaminationObservation: [this.viewModel.noseExaminationObservation],
      noseExaminationOther: [this.viewModel.noseExaminationOther],
      earExaminationTest: [this.viewModel.earExaminationTest],
      earExaminationObservation: [this.viewModel.earExaminationObservation],
      earExaminationOther: [this.viewModel.earExaminationOther],
      trunkExaminationTest: [this.viewModel.trunkExaminationTest],
      trunkExaminationObservation: [this.viewModel.trunkExaminationObservation],
      trunkExaminationOther: [this.viewModel.trunkExaminationOther],
      abdomenExaminationTest: [this.viewModel.abdomenExaminationTest],
      abdomenExaminationObservation: [this.viewModel.abdomenExaminationObservation],
      abdomenExaminationOther: [this.viewModel.abdomenExaminationOther],
      memberExaminationTest: [this.viewModel.memberExaminationTest],
      memberExaminationObservation: [this.viewModel.memberExaminationObservation],
      memberExaminationOther: [this.viewModel.memberExaminationOther],
      handExaminationTest: [this.viewModel.handExaminationTest],
      handExaminationObservation: [this.viewModel.handExaminationObservation],
      handExaminationOther: [this.viewModel.handExaminationOther],
      footExaminationTest: [this.viewModel.footExaminationTest],
      footExaminationObservation: [this.viewModel.footExaminationObservation],
      footExaminationOther: [this.viewModel.footExaminationOther],
      genitoExaminationTest: [this.viewModel.genitoExaminationTest],
      genitoExaminationObservation: [this.viewModel.genitoExaminationObservation],
      genitoExaminationOther: [this.viewModel.genitoExaminationOther],
      anusExaminationTest: [this.viewModel.anusExaminationTest],
      anusExaminationObservation: [this.viewModel.anusExaminationObservation],
      anusExaminationOther: [this.viewModel.anusExaminationOther],
      columnExaminationTest: [this.viewModel.columnExaminationTest],
      columnExaminationObservation: [this.viewModel.columnExaminationObservation],
      columnExaminationOther: [this.viewModel.columnExaminationOther],
      skinExaminationTest: [this.viewModel.skinExaminationTest],
      skinExaminationOther: [this.viewModel.skinExaminationOther],
      anomaly: [this.viewModel.anomaly],
      comments: [this.viewModel.comments],
      nameClinician: [this.viewModel.nameClinician],
      currentDate: [this.viewModel.currentDate || moment()]
    });
  }

  ngAfterViewInit(): void {
    let self = this;
    if (self.id > 0) {
      self.loadData();
    }
  }

  loadData(): void {
    let self = this;
    self.setBusy(true);

    self.metaFormService.getForm(self.id).then(result => {
        let form = result as Form;

        self.identifier = form.formIdentifier;

        self.updateForm(self.viewModelForm, JSON.parse(form.formValues[0].formControlValue));
        self.updateForm(self.viewPatientModelForm, JSON.parse(form.formValues[1].formControlValue));
        self.updateForm(self.viewObstetricModelForm, JSON.parse(form.formValues[2].formControlValue));
        self.updateForm(self.viewBirthResultModelForm, JSON.parse(form.formValues[3].formControlValue));
        self.updateForm(self.viewSurfaceExamModelForm, JSON.parse(form.formValues[4].formControlValue));

        self.isComplete = form.completeStatus == 'Complete';
        self.isSynched = form.synchStatus == 'Synched';

        if(self.isComplete || self.isSynched) {
          self.viewPatientModelForm.disable();
          self.viewObstetricModelForm.disable();
          self.viewBirthResultModelForm.disable();
          self.viewSurfaceExamModelForm.disable();
        }
              
        self.setBusy(false);
    }, error => {
          self.throwError(error, error.statusText);
    });
  }  

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
    this.eventService.removeAll(FormCComponent.name);
  }    

  loadDropDowns(): void {
    let self = this;
    self.getFacilityList();
  }

  getFacilityList(): void {
    let self = this;
    self.facilityService.getAllFacilities()
        .pipe(takeUntil(self._unsubscribeAll))
        .subscribe(result => {
            self.facilityList = result;
        }, error => {
            self.throwError(error, error.statusText);
        });
  }

  openCompletePopup(formId: number) {
    let self = this;
    let title = "Form Completed";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormCompletePopupComponent, {
      width: '720px',
      disableClose: true,
      data: { formId, title: title }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
        self._router.navigate([_routes.clinical.forms.landing]);        
      })
  }  

  openGuidelinesPopup() {
    let self = this;
    let title = "GUIDELINES FOR COMPLETEING FOLLOW UP FORM FOR PREGNANT WOMEN (FORM C)";
    let dialogRef: MatDialogRef<any> = self.dialog.open(FormGuidelinesPopupComponent, {
      width: '920px',
      disableClose: true,
      data: { title: title, type: 'A' }
    })
    dialogRef.afterClosed()
      .subscribe(res => {
      })
  }   

  completeForm(): void {
    let self = this;
    let otherModels:any[];
    let attachments:FormAttachmentModel[] = [];

    otherModels = [this.viewObstetricModelForm.value, this.viewBirthResultModelForm.value, this.viewSurfaceExamModelForm.value];

    if (self.id == 0) {
      self.metaFormService.saveFormToDatabase('FormC', this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form C saved successfully!', 'Form Saved');
                this.openCompletePopup(response);
            }
            else {
                self.showError('There was an error saving form C, please try again !', 'Download');
            }
        });   
    }
    else {
      self.metaFormService.updateForm(self.id, this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form C updated successfully!', 'Form Saved');
                this.openCompletePopup(self.id);
            }
            else {
                self.showError('There was an error updating form C, please try again !', 'Download');
            }
        });   
    }
  }  

  saveForm(): void {
    let self = this;
    let otherModels:any[];
    let attachments:FormAttachmentModel[] = [];

    otherModels = [this.viewObstetricModelForm.value, this.viewBirthResultModelForm.value, this.viewSurfaceExamModelForm.value];

    if (self.id == 0) {
      self.metaFormService.saveFormToDatabase('FormC', this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form C saved successfully!', 'Form Saved');
                self._router.navigate([_routes.clinical.forms.landing]);
            }
            else {
                self.showError('There was an error saving form C, please try again !', 'Download');
            }
        });   
    }
    else {
      self.metaFormService.updateForm(self.id, this.viewModelForm.value, this.viewPatientModelForm.value, attachments, otherModels).then(response =>
        {
            if (response) {
                self.notify('Form C updated successfully!', 'Form Saved');
                self._router.navigate([_routes.clinical.forms.landing]);
            }
            else {
                self.showError('There was an error updating form C, please try again !', 'Download');
            }
        });   
    }

  }
}

class ViewModel {
  formCompleted: any;

  treatmentSiteId: string;
  visitDate: Moment;
  asmNumber: string;
  patientFirstName: string;
  patientLastName: string;

  firstPregnancy: string;
  previousChildrenAbnormality: string;
  previousChildrenAbnormalityDescription: string;
  immediateFamilyAbnormality: string;
  immediateFamilyAbnormalityWho: string;
  immediateFamilyAbnormalityDescription: string;

  deliveryDate: Moment;
  placeDelivery: string;
  placeDeliverySpecify: string;
  typeDelivery: string;
  gestAgeUntilDeliveryWeeks: number;
  gestAgeUntilDeliveryDays: number;
  methodEvaluatePregnancy: string;
  outcomePregnancy: string;
  numberBabies: string;
  numberBabiesOther: string;
  whoAttendedBirth: string;
  whoAttendedSpecify: string;
  deliveryInduced: string;
  howBabyBorn: string;
  howBabyBornSpecify: string;
  complication: string;
  complicationSpecify: string;

  babyName: string;
  babyASMNumber: string;
  babyAgeDays: number;
  babyGender: string;
  babyLength: number;
  headCircumference: number;
  birthWeight: number;
  
  headInspectionTest: string;
  headInspectionObservation: string;
  headInspectionOther: string;

  faceExaminationTest: string;
  faceExaminationObservation: string;
  faceExaminationOther: string;

  eyeExaminationTest: string;
  eyeExaminationObservation: string;
  eyeExaminationOther: string;

  mouthExaminationTest: string;
  mouthExaminationObservation: string;
  mouthExaminationOther: string;

  noseExaminationTest: string;
  noseExaminationObservation: string;
  noseExaminationOther: string;

  earExaminationTest: string;
  earExaminationObservation: string;
  earExaminationOther: string;

  trunkExaminationTest: string;
  trunkExaminationObservation: string;
  trunkExaminationOther: string;

  abdomenExaminationTest: string;
  abdomenExaminationObservation: string;
  abdomenExaminationOther: string;

  memberExaminationTest: string;
  memberExaminationObservation: string;
  memberExaminationOther: string;

  handExaminationTest: string;
  handExaminationObservation: string;
  handExaminationOther: string;

  footExaminationTest: string;
  footExaminationObservation: string;
  footExaminationOther: string;

  genitoExaminationTest: string;
  genitoExaminationObservation: string;
  genitoExaminationOther: string;

  anusExaminationTest: string;
  anusExaminationObservation: string;
  anusExaminationOther: string;

  columnExaminationTest: string;
  columnExaminationObservation: string;
  columnExaminationOther: string;

  skinExaminationTest: string;
  skinExaminationOther: string;

  anomaly: string;
  comments: string;
  nameClinician: string;
  currentDate: Moment;
}