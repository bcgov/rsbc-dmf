import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakOptions, KeycloakService } from 'keycloak-angular';
import { AuthRoutes } from '../../features/auth/auth.routes';
import { ConfigurationService } from '../../shared/services/configuration.service';

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
    const authenticated = await this.keycloakService.init(
      this.getKeycloakOptions()
    );

    this.keycloakService.getKeycloakInstance().onTokenExpired = (): void => {
      this.keycloakService
        .updateToken()
        .catch(() => this.router.navigateByUrl(AuthRoutes.MODULE_PATH));
    };

    if (authenticated) {
      // Force refresh to begin expiry timer
      await this.keycloakService.updateToken(-1);
    }
  }

  private getKeycloakOptions(): KeycloakOptions {
    console.info('config', this.configService.getKeycloakOptions());
    return this.configService.getKeycloakOptions();
  }
}
