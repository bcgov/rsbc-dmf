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
  public config: AppConfiguration = { keycloak: { } };

  constructor(
    private configurationService: ConfigService
  ) {}

  public load(): Observable<AppConfiguration> {
    console.info("Loading configuration...");
    return this.configurationService.apiConfigGet$Json().pipe(
      tap((appConfiguration: AppConfiguration) => {
        console.info("Configuration loaded:", appConfiguration);
        this.config = appConfiguration;
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
