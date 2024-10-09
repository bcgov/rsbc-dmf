import { Injectable } from '@angular/core';
import { Observable, from, of } from 'rxjs';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakLoginOptions } from 'keycloak-js';

export interface IAuthService {
  login(options?: KeycloakLoginOptions): Observable<void>;
  logout(redirectUri: string): Observable<void>;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService implements IAuthService {
  public constructor(private keycloakService: KeycloakService) {}

  public login(options?: KeycloakLoginOptions): Observable<void> {
    return from(this.keycloakService.login(options));
  }

  public getDisplayName(): string | undefined {
    const idTokenParsed = this.keycloakService.getKeycloakInstance().idTokenParsed;
    if (idTokenParsed !== undefined) {
      return idTokenParsed['preferred_username'];
    }
    return undefined;
  }

  public getRoles(): string[] {
    return this.keycloakService.getUserRoles()
  }

  public logout(redirectUri: string): Observable<void> {
    return from(this.keycloakService.logout(redirectUri));
  }
}
