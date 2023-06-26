import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { _events } from '../../../config/events';
import { BaseComponent } from 'app/shared/base/base.component';
import { egretAnimations } from 'app/shared/animations/egret-animations';

@Component({
    selector: 'error-403',
    templateUrl: './error.403.component.html',
    styleUrls: ['./error.403.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: egretAnimations
})
export class Error403Component extends BaseComponent implements OnInit {
    public viewModel: ViewModel;

    ngOnInit(): void {
        let self = this;
        if (history.state && history.state.error) {
            self.viewModel = new ViewModel(history.state.error);
        } else {
            self.viewModel = new ViewModel(null);
        }
    }
}

class ViewModel {
    constructor(error: any) {
        if (error) {
            this.statusCode = error.statusCode;
            this.statusCodeType = error.statusCodeType;
            this.message = error.message;
            this.referenceCode = error.referenceCode;
        }
    }

    statusCode: number;
    statusCodeType: string;
    message: string;
    referenceCode: string;
}