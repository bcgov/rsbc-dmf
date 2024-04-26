import { enableProdMode, importProvidersFrom } from '@angular/core';
import { environment } from './environments/environment';
import { AppComponent } from './app/app.component';
import { ApiModule } from './app/shared/api/api.module';
import { provideRouter } from '@angular/router';
import { OAuthModule } from 'angular-oauth2-oidc';
import { LayoutModule, CoreUiModule } from '@shared/core-ui';
import { FormsModule } from '@angular/forms';
import { withInterceptorsFromDi, provideHttpClient } from '@angular/common/http';
import { AppRoutingModule } from './app/app-routing.module';
import { provideAnimations } from '@angular/platform-browser/animations';
import { BrowserModule, bootstrapApplication } from '@angular/platform-browser';
import { APP_BASE_HREF, PlatformLocation } from '@angular/common';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

// const providers = [
//   { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] }
// ];

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
    providers: [
        importProvidersFrom(BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }), AppRoutingModule, FormsModule, LayoutModule, OAuthModule.forRoot({
            resourceServer: {
                sendAccessToken: true,
                customUrlValidation: (url) => url.toLowerCase().includes('/api/') &&
                    !url.toLowerCase().endsWith('/config'),
            },
        }), CoreUiModule, AppRoutingModule, ApiModule.forRoot({ rootUrl: environment.apiRootUrl })),
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
        provideAnimations(),
        provideHttpClient(withInterceptorsFromDi()),
        provideRouter([]),
    ]
})
  .catch(err => console.log(err));
