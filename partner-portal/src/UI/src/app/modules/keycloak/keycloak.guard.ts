import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router,
} from '@angular/router';
import { IdentityProvider } from '@shared/core-ui';
import { ConfigurationService } from '@app/shared/services/configuration.service';
import { KeycloakAuthGuard, KeycloakService } from 'keycloak-angular';
import { ProfileService } from '@app/shared/api/services';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { UserService } from '@app/shared/services/user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard extends KeycloakAuthGuard {
  constructor(
    protected override readonly router: Router,
    protected readonly keycloakService: KeycloakService,
    private readonly configService: ConfigurationService,
    private profileService: ProfileService,
    private userService: UserService,
  ) {
    super(router, keycloakService);
  }

  async isAccessAllowed(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
  ): Promise<boolean | UrlTree> {
    if (!this.authenticated) {
      const scope =
        this.configService.config.keycloak?.config?.scope ||
        'openid profile email';
      try {

        await this.keycloakService.login({
          idpHint: IdentityProvider.IDIR,
          scope: scope,
        });

      } catch (error) {
        console.error('Keyclock: Login failed', error);
      }
    }

    if (this.authenticated) {
      try {
        await firstValueFrom(this.profileService.apiProfileCurrentGet$Json());
      } catch (error) {
        console.error('Login failed', error);
        // if (error && (error as HttpErrorResponse)?.status === 400 && ((error as HttpErrorResponse).error as string).includes('User not found')) {
        // Handle unauthorized access, possibly redirect to User Access Request page
        console.warn('Unauthorized access - redirecting to User Access.');
        this.router.navigate(['/userAccess']);

        return false;
        //}
      }

      this.hasUserAccess().then((hasAccess) => {
        if (hasAccess == false) {
          this.router.navigate(['restrictedAccess/unauthorizedUser']);
        }
      });
      this.hasUserExpired().then((isExpired) => {
        if (isExpired) {
          this.router.navigate(['restrictedAccess/expiredUser']);
        }
      });
    }

    console.log('User is authenticated:', this.authenticated);
    return this.authenticated;
  }

  async hasUserAccess(): Promise<boolean> {
    const userDetails = await firstValueFrom(
      this.userService.getCurrentLoginDetails(),
    );
    return userDetails.userRoles?.some((x) => x.includes('User')) ?? false;
  }

  async hasUserExpired(): Promise<boolean> {
    const userDetails = await firstValueFrom(
      this.userService.getCurrentLoginDetails(),
    );
    return userDetails.expiryDate
      ? new Date(userDetails.expiryDate) < new Date()
      : true;
  }
}
