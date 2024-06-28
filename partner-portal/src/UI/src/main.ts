import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';


import { importProvidersFrom } from '@angular/core';
import { AppComponent } from './app/app.component';
import { AppRoutingModule } from './app/app-routing.module';
import { withInterceptorsFromDi, provideHttpClient } from '@angular/common/http';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { KeycloakModule } from './app/modules/keycloak/keycloak.module';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';


bootstrapApplication(AppComponent, {
    providers: [
        importProvidersFrom(BrowserModule, AppRoutingModule, KeycloakModule),
        provideHttpClient(withInterceptorsFromDi()),
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
})
  .catch(err => console.error(err));
