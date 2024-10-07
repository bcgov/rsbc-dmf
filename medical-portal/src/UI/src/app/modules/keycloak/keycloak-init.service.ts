import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakOptions, KeycloakService } from 'keycloak-angular';
import { AuthRoutes } from '../../features/auth/auth.routes';
import { ConfigurationService } from '../../shared/services/configuration.service';
import { ProfileManagementService } from '@app/shared/services/profile.service';
import { firstValueFrom, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class KeycloakInitService {
  public constructor(
    private configService: ConfigurationService,
    private router: Router,
    private keycloakService: KeycloakService,
    private profileManagementService: ProfileManagementService
  ) {}

  public load() {
    console.info('Keycloak initializing...');
    return this.configService.load().pipe(switchMap<any, any>(async (appConfiguration) => {
      const authenticated = await this.keycloakService.init(appConfiguration.keycloak as KeycloakOptions);
      console.info('Keycloak authenticated:', authenticated);

      this.keycloakService.getKeycloakInstance().onTokenExpired = (): void => {
        console.info('Keycloak token expired, updating token');
        this.keycloakService
          .updateToken()
          .catch((reason: any) => {
            console.error('Keycloak failed to update token', reason);
            this.router.navigateByUrl(AuthRoutes.MODULE_PATH)
          });
      };

      if (authenticated) {
        // Force refresh to begin expiry timer
        await this.keycloakService.updateToken(-1);
        // Cache profile, await for it to complete so that we can guarantee the profile is loaded before AuthGuard checks access
        await firstValueFrom(this.profileManagementService.cacheProfile());
      }

      console.info('Keycloak initialization completed.');
    }));
  }
}
