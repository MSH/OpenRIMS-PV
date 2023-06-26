import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AccountService } from '../account.service';

@Injectable()
export class AuthGuard implements CanActivate {
  public authToken;
  private isAuthenticated = true; // Set this value dynamically
  
  constructor(private router: Router,
    protected accountService: AccountService) {}
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    let self = this;
    if (self.accountService.hasToken()) {
      return true;
    }
    this.router.navigate(['/security/login']);
    return false;
  }
}