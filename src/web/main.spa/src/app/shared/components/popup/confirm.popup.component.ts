import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'cofirm-popup',
    template: `
    <h1 matDialogTitle class="mb-05">{{ data.title | translate }}</h1>
    <div mat-dialog-content class="mb-1">{{ data.message | translate }}</div>
    <div mat-dialog-actions>
        <button type="button" mat-raised-button Color="primary" (click)="dialogRef.close(true)" class="mb-12">{{'OK' | translate }}</button>&nbsp;<span fxFlex></span>
        <button type="button" color="accent" mat-raised-button (click)="dialogRef.close(false)" class="mb-12">{{'Cancel' | translate }}</button>
    </div>`,
})
export class ConfirmPopupComponent {

    constructor(public dialogRef: MatDialogRef<ConfirmPopupComponent>,
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
