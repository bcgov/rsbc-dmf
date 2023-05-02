const PROXY_CONFIG = [
  {
    context: ['/apis'],
    target: 'http://localhost:3020',
    pathRewrite: {
      '/api': 'http://localhost:3020/api',
    },
    secure: false,
    logLevel: 'debug',
  },
  {
    context: ['/apis'],
    target: 'http://localhost:5050',
    secure: false,
    logLevel: 'debug',
  },
];

module.exports = PROXY_CONFIG;
