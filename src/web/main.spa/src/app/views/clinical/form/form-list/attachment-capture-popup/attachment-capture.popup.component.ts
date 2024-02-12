import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { Subject, Observable } from 'rxjs';
import { WebcamImage, WebcamInitError, WebcamUtil } from 'ngx-webcam';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { MetaFormService } from 'app/shared/services/meta-form.service';
import { PopupService } from 'app/shared/services/popup.service';
import { _routes } from 'app/config/routes';
import { BasePopupComponent } from 'app/shared/base/base.popup.component';
import { Router } from '@angular/router';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  templateUrl: './attachment-capture.popup.component.html',
  styleUrls: ['./attachment-capture.popup.component.scss'], 
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations  
})
export class AttachmentCapturePopupComponent extends BasePopupComponent implements OnInit {


  // toggle webcam on/off
  public showWebcam = true;
  public allowCameraSwitch = true;
  public multipleWebcamsAvailable = false;
  public deviceId: string;
  public videoOptions: MediaTrackConstraints = {
    // width: {ideal: 1024},
    // height: {ideal: 576}
  };
  public errors: WebcamInitError[] = [];

  // latest snapshot
  public webcamImage: WebcamImage = null;

  // webcam snapshot trigger
  private trigger: Subject<void> = new Subject<void>();
  // switch to next / previous / specific webcam; true/false: forward/backwards, string: deviceId
  private nextWebcam: Subject<boolean|string> = new Subject<boolean|string>();
      
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ImageCapturePopupData,
    public dialogRef: MatDialogRef<AttachmentCapturePopupComponent>,
    protected _router: Router,
    protected _location: Location,
    protected accountService: AccountService,
    protected metaFormService: MetaFormService,
    protected popupService: PopupService,
  ) { 
    super(_router, _location, popupService, accountService);
  }

  ngOnInit(): void {
    WebcamUtil.getAvailableVideoInputs()
      .then((mediaDevices: MediaDeviceInfo[]) => {
        this.multipleWebcamsAvailable = mediaDevices && mediaDevices.length > 1;
      });    
  }

  public triggerSnapshot(): void {
    this.trigger.next();
  }

  public toggleWebcam(): void {
    this.showWebcam = !this.showWebcam;
  }

  public handleInitError(error: WebcamInitError): void {
    this.errors.push(error);
  }

  public showNextWebcam(directionOrDeviceId: boolean|string): void {
    // true => move forward through devices
    // false => move backwards through devices
    // string => move to device with given deviceId
    this.nextWebcam.next(directionOrDeviceId);
  }

  public handleImage(webcamImage: WebcamImage): void {
    console.info('received webcam image', webcamImage);
    this.webcamImage = webcamImage;
  }

  public cameraWasSwitched(deviceId: string): void {
    console.log('active device: ' + deviceId);
    this.deviceId = deviceId;
  }

  public get triggerObservable(): Observable<void> {
    return this.trigger.asObservable();
  }

  public get nextWebcamObservable(): Observable<boolean|string> {
    return this.nextWebcam.asObservable();
  }

  saveImage(): void {
    let self = this;

    self.triggerSnapshot();

    self.metaFormService.updateAttachment(self.data.formId, this.webcamImage.imageAsDataUrl, self.data.index).then(response =>
      {
          if (response) {
              self.notify('Image saved successfully!', 'Form Saved');
              self.dialogRef.close("Saved");
          }
          else {
              self.showError('There was an error saving the image, please try again !', 'Save Image');
          }
      });   

  }
}

export interface ImageCapturePopupData {
  formId: number;
  title: string;
  index: number;
}