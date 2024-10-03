import { APP_BASE_HREF } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { ConfigService } from '../api/services';
import { AppConfiguration, KeycloakConfiguration } from '../api/models';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  //public onLoaded: BehaviorSubject<boolean> = new BehaviorSubject(false);
  private config?: AppConfiguration;

  constructor(
    private configurationService: ConfigService
  ) {}

  public load(): Observable<KeycloakConfiguration> {
    return this.configurationService.apiConfigGet$Json().pipe(
      map((appConfiguration: AppConfiguration) => {
        this.config = appConfiguration;
        return appConfiguration.keycloak as KeycloakConfiguration;
      })
    );
  }

  // public isConfigured(): boolean {
  //   return this.config !== null;
  // }

  // public getKeycloakOptions(): KeycloakOptions {
  //   return this.config?.keycloakOptions || {};
  // }
}
