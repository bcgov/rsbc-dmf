import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApiModule } from './shared/api/api.module';
import { environment } from '../environments/environment';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';
import { BearerTokenInterceptor } from '@shared/core-ui';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ApiLoaderInterceptor } from './features/auth/interceptors/loading.interceptor';
import { AuthService } from '@shared/core-ui';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideKeycloak } from './modules/keycloak/keycloak.provider';

export const appConfig: ApplicationConfig = {
  providers: [
    provideKeycloak(),
    provideRouter(routes, withComponentInputBinding()),
    provideAnimationsAsync(),
    provideHttpClient(
      withInterceptors([BearerTokenInterceptor, ApiLoaderInterceptor]),
    ),
    importProvidersFrom(
      BrowserModule,
      BrowserAnimationsModule,
      ApiModule.forRoot({ rootUrl: environment.apiRootUrl }),
      NgxSpinnerModule,
    ),
    AuthService,
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
  ],
};
