import { KeycloakOptions } from 'keycloak-angular';

import { environmentName } from './environment.model';

export interface EnvironmentConfig {
  pidpAdapterApiEndpoint: string;
  medicalPortalApiEndpoint: string;
  environmentName: environmentName;
  applicationUrl: string;
  pidpPortalUrl: string;
  keycloakConfig: KeycloakOptions;
  featureFlags: { [key: string]: boolean };
}