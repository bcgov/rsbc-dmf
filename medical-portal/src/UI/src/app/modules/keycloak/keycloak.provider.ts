import { APP_INITIALIZER, importProvidersFrom, Provider } from "@angular/core";
import { ConfigurationService } from '../../shared/services/configuration.service';
import { KeycloakInitService } from "./keycloak-init.service";
import { KeycloakAngularModule } from "keycloak-angular";
import { switchMap } from "rxjs";

export function keycloakFactory(configService: ConfigurationService, keycloakInitService: KeycloakInitService)
{
  return () => configService
    .load()
    .pipe(
      switchMap<any, any>(
        async (appConfiguration) => keycloakInitService.load(appConfiguration)
    ));
}

export function provideKeycloak(): Provider
{
  return [
    {
      provide: APP_INITIALIZER,
      useFactory: keycloakFactory,
      multi: true,
      deps: [ConfigurationService, KeycloakInitService],
    },
    importProvidersFrom(KeycloakAngularModule)
  ]
}
