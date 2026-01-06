import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router,
} from '@angular/router';
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
    const roles = await firstValueFrom(this.userService.getCurrentLoginDetails());

    if (roles.some(x => x.includes('Admin'))) {
      return true; // allow access
    } else {
      return this.router.parseUrl('/search'); // redirect if not admin
    }
  }

  async hasAdminAccess(): Promise<boolean> {
    const roles = await firstValueFrom(this.userService.getCurrentLoginDetails());
    return roles.some(x => x.includes('Admin'));
  }

  async hasUserAccess(): Promise<boolean> {
    const roles = await firstValueFrom(this.userService.getCurrentLoginDetails());
    return roles.some(x => x.includes('User'));
  }
}