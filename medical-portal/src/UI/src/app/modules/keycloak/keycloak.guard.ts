import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { IdentityProvider } from '@app/features/auth/enums/identity-provider.enum';
import { ConfigurationService } from '@app/shared/services/configuration.service';
import { KeycloakAuthGuard, KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard extends KeycloakAuthGuard {

  constructor(
    protected override readonly router: Router,
    protected readonly keycloakService: KeycloakService,
    private readonly configService: ConfigurationService
  ) {
    super(router, keycloakService);
  }

  async isAccessAllowed(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean | UrlTree>
  {
    if (!this.authenticated) {
      const scope = this.configService.config.keycloak?.config?.scope || "openid profile email";
      await this.keycloakService.login({
        idpHint: IdentityProvider.BCSC,
        scope: scope,
      });
    }
    return this.authenticated;
  }
}
