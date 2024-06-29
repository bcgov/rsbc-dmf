import { Injectable } from '@angular/core';
import { Observable, from, of } from 'rxjs';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakLoginOptions } from 'keycloak-js';
import { Role } from '../enums/identity-provider.enum';
import { ProfileManagementService } from '@app/shared/services/profile.service';

export interface IAuthService {
  login(options?: KeycloakLoginOptions): Observable<void>;
  isLoggedIn(): Observable<boolean>;
  logout(redirectUri: string): Observable<void>;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService implements IAuthService {
  public constructor(
    private keycloakService: KeycloakService,
    private profileManagementService: ProfileManagementService
  ) {}

  public login(options?: KeycloakLoginOptions): Observable<void> {
    return from(this.keycloakService.login(options));
  }

  public getHpdid(): string | undefined {
    const idTokenParsed = this.keycloakService.getKeycloakInstance().idTokenParsed;
    if (idTokenParsed !== undefined) {
      return idTokenParsed['preferred_username'];
    }
    return undefined;
  }

  public isLoggedIn(): Observable<boolean> {
    return of(this.keycloakService.isLoggedIn());
  }

  public hasAccess(): boolean {
    //console.info('getUserRoles', this.keycloakService.getUserRoles());
    //return this.keycloakService.isLoggedIn() && this.keycloakService.isUserInRole(Role.Enrolled);
    return !!this.profileManagementService.getCachedProfile().roles?.includes(Role.Enrolled);
  }

  public getRoles(): Role[] {
    const roleNames = this.keycloakService
      .getUserRoles()
      .filter((role) => role !== Role.Enrolled);
    return roleNames
      .map((role) => Object.values(Role).find((value) => value === role))
      .filter((role) => role !== undefined) as Role[];
  }

  public logout(redirectUri: string): Observable<void> {
    sessionStorage.removeItem("profile");
    return from(this.keycloakService.logout(redirectUri));
  }
}
