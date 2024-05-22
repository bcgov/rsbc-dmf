import { APP_BASE_HREF } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '@src/environments/environment.prod';
import { Configuration } from '../api/models';
import { ConfigService } from '../api/services';
import { KeycloakOptions } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  private config: Configuration | null = null;
  private keycloakOptions: KeycloakOptions | null = null;

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

  public getKeycloakOptions(): KeycloakOptions {
    if (!this.keycloakOptions) {
      this.keycloakOptions = environment.keycloakOptions as KeycloakOptions;
    }
    return this.keycloakOptions || {};
  }
}
