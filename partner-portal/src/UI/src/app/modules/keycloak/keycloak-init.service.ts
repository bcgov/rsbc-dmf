import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakOptions, KeycloakService } from 'keycloak-angular';
import { ConfigurationService } from '../../shared/services/configuration.service';
// TODO get keycloakOptions from api/Config
import { environment } from 'src/environments/environment.prod';

@Injectable({
  providedIn: 'root',
})
export class KeycloakInitService {
  public constructor(
    private configService: ConfigurationService,
    private router: Router,
    private keycloakService: KeycloakService
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
          // TODO
          this.router.navigateByUrl('')
        });
    };

    if (authenticated) {
      // Force refresh to begin expiry timer
      await this.keycloakService.updateToken(-1);
    }

    console.info('Keycloak initialization completed.');
  }

  private getKeycloakOptions(): KeycloakOptions {
    //console.info('getKeycloakOptions', this.configService.getKeycloakOptions());
    //return this.configService.getKeycloakOptions();
    return environment.keycloakOptions as KeycloakOptions;
  }
}
