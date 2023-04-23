import { AppEnvironment, EnvironmentName } from './environment.model';
export const environment: AppEnvironment = {
  production: true,
  medicalPortalApiEndpoint: 'http://localhost:3020',
  pidpAdapterApiEndpoint: 'https://localhost:7215',
  environmentName: EnvironmentName.LOCAL,
  applicationUrl: 'http://localhost:4200',
  featureFlags: {
    isLayoutV2Enabled: false,
  },
  keycloakConfig: {
    config: {
      url: 'https://test.healthprovideridentityportal.gov.bc.ca/auth',
      realm: 'moh_applications',
      clientId: 'DMFT-WEBAPP',
    },
    initOptions: {
      onLoad: 'check-sso',
    },
  },
};
