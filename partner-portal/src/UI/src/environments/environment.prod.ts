export const environment = {
  production: true,
  apiRootUrl: '/partner-portal',
  // TODO remove this and use api/Config to get keycloak options, move the initOptions to AppSettings.json if they don't already exist
  keycloakOptions: {
    config: {
      url: 'https://dev.common-sso.justice.gov.bc.ca/auth',
      realm: 'ISB',
      clientId: 'RSBC-DMF-PartnerPortal-Web'
    },
    initOptions: {
      onLoad: 'check-sso',
      flow: 'standard',
      responseMode: 'fragment',
      pkceMethod: 'S256',
      checkLoginIframe: false
    },
  },
};
