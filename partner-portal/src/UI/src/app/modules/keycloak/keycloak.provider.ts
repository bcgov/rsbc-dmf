import { APP_INITIALIZER, importProvidersFrom, Provider } from "@angular/core";
import { ConfigurationService } from '../../shared/services/configuration.service';
import { KeycloakAngularModule } from "keycloak-angular";
import { KeycloakService } from "keycloak-angular";
import { switchMap } from "rxjs";
import { KeycloakInitService } from "@shared/core-ui";

export function keycloakFactory(configService: ConfigurationService, keycloakInitService: KeycloakInitService, keycloakService: KeycloakService)
{
  return () => configService
    .load()
    .pipe(
      switchMap<any, any>(
        async (appConfiguration) => {
          const authenticated = await keycloakInitService.load(appConfiguration);
          keycloakService.getKeycloakInstance().onTokenExpired = () => {
            keycloakService.updateToken().catch((reason: unknown) => {
              console.error('Keycloak failed to update token', reason);
              keycloakService.logout(window.location.href).catch((logoutError: unknown) => {
                console.error('Keycloak logout failed after token expiry', logoutError);
                window.location.reload();
              });
            });
          };
          return authenticated;
        }
    ));
}

export function provideKeycloak(): Provider
{
  return [
    {
      provide: APP_INITIALIZER,
      useFactory: keycloakFactory,
      multi: true,
      deps: [ConfigurationService, KeycloakInitService, KeycloakService],
    },
    importProvidersFrom(KeycloakAngularModule)
  ]
}
