export const environment = {
  production: true,
  apiRootUrl: '/medical-portal',
  // TODO remove this and use api/Config to get keycloak options, move the initOptions to AppSettings.json if they don't already exist
  keycloakOptions: {
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
