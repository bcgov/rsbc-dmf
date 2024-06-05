import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApiModule } from './shared/api/api.module';
import { environment } from '../environments/environment';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';
import { KeycloakModule } from './modules/keycloak/keycloak.module';
import { PermissionsModule } from './modules/permissions/permissions.module';
import { BearerTokenInterceptor } from './features/auth/interceptors/bearer-token.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ApiLoaderInterceptor } from './features/auth/interceptors/loading.interceptor';
import { AuthService } from './features/auth/services/auth.service';
import { BrowserModule } from '@angular/platform-browser';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withComponentInputBinding()),
    provideAnimationsAsync(),
    provideHttpClient(
      withInterceptors([BearerTokenInterceptor, ApiLoaderInterceptor]),
    ),
    importProvidersFrom(
      BrowserModule,
      KeycloakModule,
      PermissionsModule.forRoot(),
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
