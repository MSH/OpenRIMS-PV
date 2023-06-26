import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { environment } from "environments/environment";
import { LayoutService } from 'app/shared/services/layout.service';

@Component({
  templateUrl: './about.popup.component.html',
  styleUrls: ['./about.popup.component.scss']
})
export class AboutPopupComponent implements OnInit {

  version = '';
  installationDate = '';

  constructor(
    public dialogRef: MatDialogRef<AboutPopupComponent>,
    public layout: LayoutService,
  ) 
  { 
    let self = this;
    self.version = environment.appVersion;
    self.installationDate = environment.installationDate;
  }

  ngOnInit() {
  }

}
