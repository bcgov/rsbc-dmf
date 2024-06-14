import { APP_BASE_HREF } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
//import { environment } from '@src/environments/environment.prod';
// import { Configuration } from '../api/models';
import { ConfigService } from '../api/services';
//import { KeycloakOptions } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  public onLoaded: BehaviorSubject<boolean> = new BehaviorSubject(false);
  private config: unknown | null = null;

  constructor(
    @Inject(APP_BASE_HREF) public baseHref: string,
    private configurationService: ConfigService
  ) {}

  public load(): Observable<unknown> {
    if (this.config != null) {
      return of(this.config);
    }
    return this.configurationService.apiConfigGet().pipe(
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
