import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { from, Observable, of } from 'rxjs';
import { concatMap, map, tap } from 'rxjs/operators';
import { ConfigurationService } from './configuration.service';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  constructor(
    private oauthService: OAuthService,
    private configService: ConfigurationService
  ) { }

  public login(returnUrl: string = '/'): Observable<string> {
    console.debug('login', returnUrl);

    return this.configService.getOAuthConfig().pipe(
      tap(config => {
        this.oauthService.configure(config);
        this.oauthService.setupAutomaticSilentRefresh();
        console.debug('oauth service configured');
      }), concatMap(() => {
        console.debug('try login');
        return this.oauthService.loadDiscoveryDocumentAndLogin({ state: returnUrl })
          .then(loggedIn => !loggedIn ? '' : this.oauthService.state || returnUrl);
      }));
  }
  public logout(): Observable<boolean> {
    console.log('logout');

    return from(this.oauthService.revokeTokenAndLogout());
  }

  public isLoggedIn(): boolean {
    return this.oauthService.hasValidIdToken();
  }

  public getUserSession(): string {
    return btoa(this.oauthService.getAccessToken());
  }

  public getUserProfile(): IUserProfile {
    return {
      firstName: 'First',
      lastName: 'Last',
      role: 'Role'
    };
  }
}

export interface IUserProfile {
  firstName: string,
  lastName: string,
  role: string
}
