export const environment = {
  production: true,
  apiRootUrl: '/medical-portal',
  keycloakOptions: {
    config: {
      url: 'https://common-logon-test.hlth.gov.bc.ca/auth',
      realm: 'moh_applications',
      clientId: 'DMFT-WEBAPP',
    },
    initOptions: {
    },
    },
};
