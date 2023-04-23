import { Injectable } from '@angular/core';

import { Observable, from, map } from 'rxjs';

import { JwtHelperService } from '@auth0/angular-jwt';
import { KeycloakService } from 'keycloak-angular';

import { AccessTokenParsed } from '../models/access-token-parsed.model';
import { BrokerProfile } from '../models/broker-profile.model';
import { KeycloakProfile } from 'keycloak-js';

export interface IAccessTokenService {
  token(): Observable<string>;
  isTokenExpired(): boolean;
  decodeToken(): Observable<AccessTokenParsed | null>;
  roles(): string[];
  clearToken(): void;
}

@Injectable({
  providedIn: 'root',
})
export class AccessTokenService implements IAccessTokenService {
  private jwtHelper: JwtHelperService;

  public constructor(private keycloakService: KeycloakService) {
    this.jwtHelper = new JwtHelperService();
  }

  public token(): Observable<string> {
    return from(this.keycloakService.getToken());
  }

  public isTokenExpired(): boolean {
    return this.keycloakService.isTokenExpired();
  }

  public decodeToken(): Observable<AccessTokenParsed> {
    return this.token().pipe(
      map((token: string) => this.jwtHelper.decodeToken(token))
    );
  }

  public loadBrokerProfile(forceReload?: boolean): Observable<BrokerProfile> {
    return from(this.keycloakService.loadUserProfile(forceReload)).pipe(
      map((profile: KeycloakProfile) => {
        const brokerProfile: BrokerProfile = {
          firstName: profile.firstName!,
          lastName: profile.lastName!,
          email: profile.email!,
          gender: profile.attributes['gender'],
          birthdate: profile.attributes['gender'],
          username: profile.username!,
          attributes: profile.attributes
        };
        return brokerProfile;

      })
    );
  }


  public roles(): string[] {
    return this.keycloakService.getUserRoles();
  }

  public clearToken(): void {
    this.keycloakService.clearToken();
  }
}
