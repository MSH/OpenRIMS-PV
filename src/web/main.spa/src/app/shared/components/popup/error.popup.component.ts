import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'cofirm-popup',
    template: `
    <h1 matDialogTitle class="mb-05">{{ data.title | translate }}</h1>
    <div mat-dialog-content class="mb-1"><p>{{ data.message | translate }}</p>{{ data.code | translate }}</div>
    <div mat-dialog-actions>
        <button type="button" mat-raised-button Color="warn" (click)="dialogRef.close(true)" class="mb-12">{{'Close' | translate }}</button>
    </div>`,
})
export class ErrorPopupComponent {

    constructor(public dialogRef: MatDialogRef<ErrorPopupComponent>,
        @Inject(MAT_DIALOG_DATA) public data: ErrorPopupData) {
        setTimeout(() => { dialogRef.close(); }, 60000);
    }

    proceed(): void {
        this.dialogRef.close(true);
    }
}

export interface ErrorPopupData {
    title: string;
    message: string;
    code: string;
}
