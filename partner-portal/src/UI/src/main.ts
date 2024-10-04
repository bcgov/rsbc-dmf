import { APP_INITIALIZER, importProvidersFrom } from '@angular/core';
import { AppComponent } from './app/app.component';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { routes } from './app/app.routes';
import { ApiModule } from '@app/shared/api/api.module';
import { environment } from './environments/environment';
import { BearerTokenInterceptor } from '@app/features/auth/interceptors/bearer-token.interceptor';
import { KeycloakInitService } from '@app/modules/keycloak/keycloak-init.service';
import { KeycloakAngularModule } from 'keycloak-angular';
import { provideKeycloak } from './app/modules/keycloak/keycloak.provider';

export function keycloakFactory(keycloakInitService: KeycloakInitService)
{
  return () => keycloakInitService.load();
}

bootstrapApplication(AppComponent, {
  providers: [
    provideKeycloak(),
    provideRouter(routes, withComponentInputBinding()),
    importProvidersFrom(
      BrowserModule,
      KeycloakAngularModule,
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
