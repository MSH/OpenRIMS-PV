import { Component, OnInit, Inject, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { egretAnimations } from 'app/shared/animations/egret-animations';

@Component({
  selector: 'viewerror-popup',
  templateUrl: './viewerror.popup.component.html',
  encapsulation: ViewEncapsulation.None,
  animations: egretAnimations
})
export class ViewErrorPopupComponent implements OnInit {
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ViewErrorPopupData,
    public dialogRef: MatDialogRef<ViewErrorPopupComponent>,
  ) { }

  ngOnInit(): void {
  }
}

export interface ViewErrorPopupData {
  title: string;
  messages: string[];
}