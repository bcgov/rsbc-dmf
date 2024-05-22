import { KeycloakProfile } from 'keycloak-js';

export interface BrokerProfile extends KeycloakProfile {
  firstName: string;
  lastName: string;
  birthdate: string;
  gender: string;
  username: string;
  email: string;
  attributes: {
    birthdate: string;
    gender: string;
  };
}
