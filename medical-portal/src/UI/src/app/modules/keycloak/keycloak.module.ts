import { APP_INITIALIZER, NgModule, Provider } from '@angular/core';
import {KeycloakAngularModule, KeycloakService} from 'keycloak-angular';
import { KeycloakInitService } from './keycloak-init.service';

export function keycloakFactory(
  keycloakInitService: KeycloakInitService
): () => Promise<void> {
  return (): Promise<void> => keycloakInitService.load();
}

export const keycloakProvider: Provider = {
  provide: APP_INITIALIZER,
  useFactory: keycloakFactory,
  multi: true,
  deps: [KeycloakInitService],
};

// NOTE Keycloak seems to have unobvious lifecycle issues when replacing this NgModule with a standalone version. Later, it might be worth trying an updated version of Keycloak
// Stackoverflow has posts with information on how to get keycloak-angular working with standalone
@NgModule({
  imports: [KeycloakAngularModule],
  providers: [keycloakProvider],
})
export class KeycloakModule {}
