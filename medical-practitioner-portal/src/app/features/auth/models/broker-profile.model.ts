import { KeycloakProfile } from 'keycloak-js';

export interface BrokerProfile extends KeycloakProfile {
  birthdate: string;
  gender: string;
  attributes: {
    birthdate: string;
    gender: string;
  };
}
