/* tslint:disable */
/* eslint-disable */
import { KeycloakConfiguration } from '../models/keycloak-configuration';
export interface PublicConfiguration {
  chefsFormId?: string;
  environment?: string | null;
  keycloak?: KeycloakConfiguration;
}
