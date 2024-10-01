import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ConfigService } from '../api/services';
import { PublicConfiguration } from '../api/models';
//import { KeycloakOptions } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  public onLoaded: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public config!: PublicConfiguration;

  constructor(
    private configurationService: ConfigService
  ) {}

  public load(): Observable<PublicConfiguration> {
     if (this.config != null) {
       return of(this.config);
     }
    return this.configurationService.apiConfigGet$Json().pipe(
      tap((c: any) => {
        this.config = { ...c };
        this.onLoaded.next(true);
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
