// Karma configuration file, see link for more information
// https://karma-runner.github.io/1.0/config/configuration-file.html
//const path = require('path');

module.exports = function (config) {
  config.set({

    frameworks: ["jasmine", "karma-typescript", '@angular-devkit/build-angular'],    

    plugins: [
      require('karma-jasmine'),
      require('karma-chrome-launcher'),
      require('karma-coverage'),
      require('karma-typescript'),
      require('karma-typescript-angular2-transform'),
      require('@angular-devkit/build-angular/plugins/karma')
    ],

    preprocessors: {
        "**/*.ts": ["coverage", "karma-typescript"],        
    },

    karmaTypescriptConfig: {
        bundlerOptions: {
            entrypoints: /\.spec\.ts$/,
            transforms: [
                require("karma-typescript-angular2-transform")
            ]
        }
    },

    // configure the coverage reporter
    coverageReporter: {
    type : 'json',
    includeAllSources: false
    },

    reporters: ['progress', 'karma-typescript','coverage'],    

    // optionally, configure the reporter
    
    browsers: ['ChromeHeadless'],

    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: false,
    singleRun: true
  });
};
