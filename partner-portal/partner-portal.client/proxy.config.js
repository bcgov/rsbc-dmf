const { env } = require('process');

//const target1 = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
//  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:8080';

const target = 'http://localhost:8080/';

const PROXY_CONFIG = [
  {
    context: ['/api'],
    target: target,
    secure: false,
    headers: {
      Connection: 'Keep-Alive',
    },
  },
  {
    context: ['/patner-portal/api'],
    target: target,
    pathRewrite: {
      '^/partner-portal': '',
    },
    secure: false,
    headers: {
      Connection: 'Keep-Alive',
    },
  },
  {
    context: ['/swagger'],
    target: target,
    secure: false,
    headers: {
      Connection: 'Keep-Alive',
    },
  },
];

module.exports = PROXY_CONFIG;
