import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakOptions, KeycloakService } from 'keycloak-angular';
import { ConfigurationService } from '../../shared/services/configuration.service';
import { switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class KeycloakInitService {
  public constructor(
    private configService: ConfigurationService,
    private router: Router,
    private keycloakService: KeycloakService
  ) { }

  public load() {
    console.info('Keycloak initializing...');
    return this.configService.load().pipe(switchMap<any, any>(async (appConfiguration) => {
      const keycloakOptions = appConfiguration.keycloak as KeycloakOptions;
      console.info('Keycloak options:', keycloakOptions);
      const authenticated = await this.keycloakService.init(keycloakOptions);
      console.info('Keycloak authenticated:', authenticated);

      this.keycloakService.getKeycloakInstance().onTokenExpired = (): void => {
        console.info('Keycloak token expired, updating token');
        this.keycloakService
          .updateToken()
          .catch((reason) => {
            console.error('Keycloak failed to update token', reason);
          });
      };

      if (authenticated) {
        // Force refresh to begin expiry timer
        await this.keycloakService.updateToken(-1);
      }

      console.info('Keycloak initialization completed.');
    }));
  }
}
