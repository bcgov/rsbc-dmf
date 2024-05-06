import { APP_BASE_HREF } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { AuthConfig } from 'angular-oauth2-oidc';
import { Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
//import { environment } from 'src/environments/environment';
import { Configuration } from '../api/models';
import { ConfigService } from '../api/services';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  private config: Configuration | null = null;

  constructor(
    @Inject(APP_BASE_HREF) public baseHref: string,
    private configurationService: ConfigService
  ) {}

  public load(): Observable<Configuration> {
    if (this.config != null) {
      return of(this.config);
    }
    return this.configurationService.apiConfigGet$Json().pipe(
      tap((c) => {
        this.config = { ...c };
      })
    );
  }

  public isConfigured(): boolean {
    return this.config !== null;
  }

  public getOAuthConfig(): Observable<AuthConfig> {
    return this.load().pipe(
      map((c) => {
        return {
          requestAccessToken: true,
          issuer: c.oidcConfiguration?.issuer || undefined,
          clientId: c.oidcConfiguration?.clientId || undefined,
          requireHttps: false,
          strictDiscoveryDocumentValidation: false,
          redirectUri: window.location.origin + this.baseHref, // concat base href to the redirect URI
          responseType: 'code',
          scope: c.oidcConfiguration?.scope || undefined,
          showDebugInformation: true, //!environment.production,
          customQueryParams: {
            acr_values: 'idp:bcsc',
          },
        };
      })
    );
  }
}
