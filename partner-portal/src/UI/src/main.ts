import { importProvidersFrom } from '@angular/core';
import { AppComponent } from './app/app.component';
import {
  withInterceptorsFromDi,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { KeycloakModule } from './app/modules/keycloak/keycloak.module';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { routes } from './app/app.routes';
import { ApiModule } from '@app/shared/api/api.module';
import { environment } from './environments/environment';
import { BearerTokenInterceptor } from '@app/features/auth/interceptors/bearer-token.interceptor';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes, withComponentInputBinding()),

    importProvidersFrom(
      BrowserModule,
      KeycloakModule,
      BrowserAnimationsModule,
      ApiModule.forRoot({ rootUrl: environment.apiRootUrl }),
    ),

    provideHttpClient(withInterceptors([BearerTokenInterceptor])),
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
}).catch((err) => console.error(err));
