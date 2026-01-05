const target = 'http://127.0.0.1:8080/';

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
    context: ['/partner-portal/api'], 
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
