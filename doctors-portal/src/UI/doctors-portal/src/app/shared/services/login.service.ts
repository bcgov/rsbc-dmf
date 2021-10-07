import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { from, Observable, of } from 'rxjs';
import { concatMap, map, tap } from 'rxjs/operators';
import { UserProfile } from '../api/models';
import { ProfileService } from '../api/services';
import { ConfigurationService } from './configuration.service';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  public userProfile?: Profile = undefined;

  constructor(
    private oauthService: OAuthService,
    private configService: ConfigurationService,
    private profileService: ProfileService
  ) { }

  public login(returnUrl: string = '/'): Observable<string> {
    console.debug('login', returnUrl);

    return this.configService.getOAuthConfig().pipe(
      tap(config => {
        console.log(config);
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
    console.debug('logout');
    return from(this.oauthService.revokeTokenAndLogout());
  }

  public isLoggedIn(): boolean {
    return this.oauthService.hasValidIdToken();
  }

  public getUserSession(): string {
    return btoa(this.oauthService.getAccessToken());
  }

  public getUserProfile(): Observable<Profile> {
    return this.profileService.apiProfileCurrentGet$Json().pipe(tap(profile => {
      this.userProfile = profile;
    }));
  }
}

export interface Profile extends UserProfile { }
