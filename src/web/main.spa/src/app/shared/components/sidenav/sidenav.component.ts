import { Component, OnInit, Input } from '@angular/core';
import { NavigationService } from 'app/shared/services/navigation.service';
import { AccountService } from 'app/shared/services/account.service';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.template.html'
})
export class SidenavComponent {
  @Input('items') public menuItems: any[] = [];
  @Input('hasIconMenu') public hasIconTypeMenuItem: boolean;
  @Input('iconMenuTitle') public iconTypeMenuTitle: string;

  constructor(
    public navService: NavigationService,
    public accountService: AccountService,
  ) {}
  
  ngOnInit() 
  {
    
  }

  getRouter(data): string {
    return "/" + data.state + data.parameter != null ? data.parameter : "";
  }  

}