import { APP_INITIALIZER, importProvidersFrom, Provider } from "@angular/core";
import { KeycloakInitService } from "./keycloak-init.service";
import { KeycloakAngularModule, KeycloakService } from "keycloak-angular";
import { ConfigurationService } from "@app/shared/services/configuration.service";

export function keycloakFactory(keycloakInitService: KeycloakInitService)
{
  return () => keycloakInitService.load();
}

export function provideKeycloak(): Provider
{
  return [{
    provide: APP_INITIALIZER,
    useFactory: keycloakFactory,
    multi: true,
    deps: [KeycloakInitService, KeycloakService, ConfigurationService],
  },
  importProvidersFrom(KeycloakAngularModule)]
}
