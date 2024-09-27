/* tslint:disable */
/* eslint-disable */
import { KeycloakConfig } from '../models/keycloak-config';
import { KeycloakInitOptions } from '../models/keycloak-init-options';
export interface KeycloakConfiguration {
  config?: KeycloakConfig;
  initOptions?: KeycloakInitOptions;
  realmUrl?: string | null;
  tokenUrl?: string | null;
  wellKnownConfig?: string | null;
}
