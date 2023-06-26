import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { _events } from '../../../config/events';
import { egretAnimations } from 'app/shared/animations/egret-animations';
import { BaseComponent } from 'app/shared/base/base.component';

@Component({
    selector: 'error-general',
    templateUrl: './error.general.component.html',
    styleUrls: ['./error.general.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: egretAnimations
})
export class ErrorGeneralComponent extends BaseComponent implements OnInit {
    ngOnInit(): void {}
}
