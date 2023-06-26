import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'cofirm-popup',
    template: `
    <h1 matDialogTitle class="mb-05">{{ data.title | translate }}</h1>
    <div mat-dialog-content class="mb-1">{{ data.message | translate }}</div>
    <div mat-dialog-actions>
        <button type="button" mat-raised-button color="primary" (click)="dialogRef.close(true)" class="mb-12">{{'Close' | translate }}</button>
    </div>`,
})
export class InfoPopupComponent {

    constructor(public dialogRef: MatDialogRef<InfoPopupComponent>,
        @Inject(MAT_DIALOG_DATA) public data: InfoPopupData) {
        setTimeout(() => { dialogRef.close(); }, 60000);
    }

    proceed(): void {
        this.dialogRef.close(true);
    }
}

export interface InfoPopupData {
    title: string;
    message: string;
}
