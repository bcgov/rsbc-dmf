import { Injectable } from '@angular/core';
import { KeycloakOptions, KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class KeycloakInitService {
  public constructor(
    private keycloakService: KeycloakService,
  ) {}

  public async load(appConfiguration: any): Promise<boolean> {
    console.info('Keycloak initializing...');
      const authenticated = await this.keycloakService.init(appConfiguration.keycloak as KeycloakOptions);
      console.info('Keycloak authenticated:', authenticated);

      this.keycloakService.getKeycloakInstance().onTokenExpired = (): void => {
        console.info('Keycloak token expired, updating token');
        this.keycloakService
          .updateToken()
          .catch((reason: any) => {
            console.error('Keycloak failed to update token', reason);
          });
      };

      if (authenticated) {
        // Force refresh to begin expiry timer
        await this.keycloakService.updateToken(-1);
      }

      console.info('Keycloak initialization completed.');
      return authenticated;
  };
}
