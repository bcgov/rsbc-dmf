import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router,
} from '@angular/router';
import { CurrentLoginDetails } from '@app/shared/api/models/current-login-details';
import { UserService } from '@app/shared/services/user.service';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AdminAuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private userService: UserService,
  ) {}

  async canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean | UrlTree> {
    const userDetails = await firstValueFrom(this.userService.getCurrentLoginDetails());

    if (userDetails.userRoles?.some(x => x.includes('Admin'))) {
      return true; // allow access
    } else {
      return this.router.parseUrl('/search'); // redirect if not admin
    }
  }

  async hasAdminAccess(): Promise<boolean> {
    const userDetails = await firstValueFrom(this.userService.getCurrentLoginDetails());
    return userDetails.userRoles?.some(x => x.includes('Admin')) ?? false;
  }

  async hasUserAccess(): Promise<boolean> {
    const userDetails = await firstValueFrom(this.userService.getCurrentLoginDetails());
    return userDetails.userRoles?.some(x => x.includes('User')) ?? false;
  }

  async hasUserExpired(): Promise<boolean> {
    const userDetails = await firstValueFrom(this.userService.getCurrentLoginDetails());
    return userDetails.expiryDate ? new Date(userDetails.expiryDate) < new Date() : true;
  }
}
