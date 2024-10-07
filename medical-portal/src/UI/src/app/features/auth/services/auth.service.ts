import { Injectable } from '@angular/core';
import { Observable, from, of } from 'rxjs';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakLoginOptions } from 'keycloak-js';
import { Role } from '../enums/identity-provider.enum';
import { ProfileManagementService } from '@app/shared/services/profile.service';
import { SESSION_STORAGE_KEYS } from '@app/app.model';

export interface IAuthService {
  logout(redirectUri: string): Observable<void>;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService implements IAuthService {
  public constructor(
    private keycloakService: KeycloakService,
  ) {}

  public getHpdid(): string | undefined {
    const idTokenParsed = this.keycloakService.getKeycloakInstance().idTokenParsed;
    if (idTokenParsed !== undefined) {
      return idTokenParsed['preferred_username'];
    }
    return undefined;
  }

  public getRoles(): Role[] {
    const roleNames = this.keycloakService
      .getUserRoles()
      .filter((role) => role !== Role.Enrolled);
    // TODO roles should come from Api and Api should add MOA role, for now just use MOA role if no other role exists
    if (roleNames.length == 0)
      roleNames.push(Role.Moa);
    return roleNames
      .map((role) => Object.values(Role).find((value) => value === role))
      .filter((role) => role !== undefined) as Role[];
  }

  public logout(redirectUri: string): Observable<void> {
    sessionStorage.removeItem(SESSION_STORAGE_KEYS.PROFILE);
    return from(this.keycloakService.logout(redirectUri));
  }
}
