import { APP_BASE_HREF } from '@angular/common';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
//import { environment } from '@src/environments/environment.prod';
// import { Configuration } from '../api/models';
//import { ConfigService } from '../api/services';
//import { KeycloakOptions } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  public onLoaded: BehaviorSubject<boolean> = new BehaviorSubject(false);
  private config: unknown | null = null;

  constructor(
    @Inject(APP_BASE_HREF) public baseHref: string,
    //private configurationService: ConfigService
  ) {}

  public load() {
    if (this.config != null) {
      return of(this.config);
    }
    // TODO add backend configuration service and uncomment this code block and remove the hardcoded config below
    // return this.configurationService.apiConfigGet().pipe(
    //   tap((c: any) => {
    //     this.config = { ...c };
    //     this.onLoaded.next(true);
    //   })
    // );
    this.config =   {
      "Keycloak": {
        "RealmUrl": "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications",
        "Config": {
          "Url": "https://common-logon-test.hlth.gov.bc.ca/auth",
          "Audience": "LICENCE-STATUS",
          "Realm": "moh_applications",
          "ClientId": "DMFT-WEBAPP"
        },
        "InitOptions": {
          "OnLoad": "check-sso",
          "Flow": "standard",
          "ResponseMode": "fragment",
          "PkceMethod": "S256"
        }
      },
    };
    this.onLoaded.next(true);
    return true;
  }

  // public isConfigured(): boolean {
  //   return this.config !== null;
  // }

  // public getKeycloakOptions(): KeycloakOptions {
  //   return this.config?.keycloakOptions || {};
  // }
}
