import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { Location } from '@angular/common';
import { BaseComponent } from 'app/shared/base/base.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PopupService } from 'app/shared/services/popup.service';
import { AccountService } from 'app/shared/services/account.service';
import { EventService } from 'app/shared/services/event.service';
import { MediaObserver } from '@angular/flex-layout';
import { ProgressStatusEnum, ProgressStatus } from 'app/shared/models/program-status.model';
import { HttpEventType } from '@angular/common/http';
import { WorkFlowService } from 'app/shared/services/work-flow.service';
import { egretAnimations } from 'app/shared/animations/egret-animations';

@Component({
  templateUrl: './spontaneous-analyser.component.html',
  styleUrls: ['./spontaneous-analyser.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class SpontaneousAnalyserComponent extends BaseComponent implements OnInit, OnDestroy {

  constructor(
    protected _activatedRoute: ActivatedRoute,
    protected _router: Router,
    protected _location: Location,
    protected _formBuilder: FormBuilder,
    protected popupService: PopupService,
    protected eventService: EventService,
    protected workFlowService: WorkFlowService,
    protected mediaObserver: MediaObserver,
    public accountService: AccountService) 
  {
    super(_router, _location, popupService, accountService, eventService);
  }

  viewDatasetModelForm: FormGroup;
  
  ngOnInit(): void {
    const self = this;
    self.viewDatasetModelForm = self._formBuilder.group({
      workflowId: ['4096D0A3-45F7-4702-BDA1-76AEDE41B986']
    });    
  }

  percentage: number;
  showProgress: boolean;

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
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