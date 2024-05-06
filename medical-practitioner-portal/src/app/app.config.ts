import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { OAuthModule } from 'angular-oauth2-oidc';
import { provideHttpClient } from '@angular/common/http';
import { ApiModule } from './shared/api/api.module';
import { environment } from '../environments/environment';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes), provideAnimationsAsync(), provideHttpClient(),
    importProvidersFrom(
      OAuthModule.forRoot({
        resourceServer: {
          sendAccessToken: true,
          customUrlValidation: (url: string) => url.toLowerCase().includes('/api/') && !url.toLowerCase().endsWith('/config'),
        }
      }),
      ApiModule.forRoot({ rootUrl: environment.apiRootUrl })),
      {
        provide: APP_BASE_HREF,
          useFactory: (s: PlatformLocation) => {
              let result = s.getBaseHrefFromDOM();
              const hasTrailingSlash = result[result.length - 1] === '/';
              if (hasTrailingSlash) {
                  result = result.substr(0, result.length - 1);
              }
              return result;
          },
          deps: [PlatformLocation],
      },
    ]
};
