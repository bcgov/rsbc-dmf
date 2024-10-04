import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { IdentityProvider } from '@app/features/auth/enums/identity-provider.enum';
import { KeycloakAuthGuard, KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard extends KeycloakAuthGuard {

  constructor(
    protected override readonly router: Router,
    protected readonly keycloakService: KeycloakService
  ) {
    super(router, keycloakService);
  }

  async isAccessAllowed(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean | UrlTree>
  {
    if (!this.authenticated) {
      await this.keycloakService.login({
        idpHint: IdentityProvider.IDIR,
        // TODO add medical-portal scope and move this to api/Config
        scope: 'openid profile email',
      });
    }
    return this.authenticated;
  }
}
