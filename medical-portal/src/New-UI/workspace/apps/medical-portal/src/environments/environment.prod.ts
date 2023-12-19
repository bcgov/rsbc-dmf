import { AppEnvironment, EnvironmentName } from './environment.model';

/**
 * @description
 * Production environment populated with the default
 * environment information and appropriate overrides.
 *
 * NOTE: This environment is for local development from
 * within a container, and not used within the deployment
 * pipeline. For pipeline config mapping see main.ts and
 * the AppConfigModule.
 */
export const environment: AppEnvironment = {
  production: true,
  pidpAdapterApiEndpoint:
    'https://pidp-adapter-0137d5-dev.apps.silver.devops.gov.bc.ca/api',
  medicalPortalApiEndpoint: 'http://localhost:3020',
  environmentName: EnvironmentName.LOCAL,
  applicationUrl: 'http://localhost:4200',
  pidpPortalUrl: 'https://test.healthprovideridentityportal.gov.bc.ca',
  featureFlags: {
    isLayoutV2Enabled: false,
  },
  emails: {
    providerIdentitySupport: 'drivermedicalportal@gov.bc.ca',
  },
  urls: {
    bcscAppDownload: `https://www2.gov.bc.ca/gov/content/governments/government-id/bcservicescardapp/download-app`,
    bcscSupport: `https://www2.gov.bc.ca/gov/content/governments/government-id/bcservicescardapp/help`,
    bcscMobileSetup: 'https://id.gov.bc.ca/account',
  },
  keycloakConfig: {
    config: {
      url: 'https://common-logon-test.hlth.gov.bc.ca/auth',
      realm: 'moh_applications',
      clientId: 'DMFT-WEBAPP',
    },
    initOptions: {
      onLoad: 'check-sso',
      flow: 'standard',
      responseMode: 'fragment',
      pkceMethod: 'S256',
    },
  },
};
