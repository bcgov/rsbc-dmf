import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakOptions, KeycloakService } from 'keycloak-angular';
import { AuthRoutes } from '../../features/auth/auth.routes';
import { ConfigurationService } from '../../shared/services/configuration.service';
// TODO
import { environment } from '@src/environments/environment.prod';
import { ProfileManagementService } from '@app/shared/services/profile.service';
import { firstValueFrom } from 'rxjs';

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

  public async load(): Promise<void> {
    console.info('Keycloak initializing...');
    const authenticated = await this.keycloakService.init(
      this.getKeycloakOptions()
    );
    console.info('Keycloak authenticated:', authenticated);

    this.keycloakService.getKeycloakInstance().onTokenExpired = (): void => {
      console.info('Keycloak token expired, updating token');
      this.keycloakService
        .updateToken()
        .catch((reason) => {
          console.error('Keycloak failed to update token', reason);
          this.router.navigateByUrl(AuthRoutes.MODULE_PATH)
        });
    };

    if (authenticated) {
      // Force refresh to begin expiry timer
      await this.keycloakService.updateToken(-1);
      await firstValueFrom(this.profileManagementService.cacheProfile());
    }

    console.info('Keycloak initialization completed.');
  }

  private getKeycloakOptions(): KeycloakOptions {
    //console.info('getKeycloakOptions', this.configService.getKeycloakOptions());
    //return this.configService.getKeycloakOptions();
    return environment.keycloakOptions as KeycloakOptions;
  }
}
