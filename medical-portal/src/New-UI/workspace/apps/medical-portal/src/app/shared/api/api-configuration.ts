/* tslint:disable */

/* eslint-disable */
import { Inject, Injectable } from '@angular/core';

import { APP_CONFIG, AppConfig } from '@app/app.config';

/**
 * Global configuration
 */
@Injectable({
  providedIn: 'root',
})
export class ApiConfiguration {
  public constructor(@Inject(APP_CONFIG) config: AppConfig) {
    this.rootUrl = config.medicalPortalApiEndpoint;
  }

  rootUrl: string = '';
}

/**
 * Parameters for `ApiModule.forRoot()`
 */
export interface ApiConfigurationParams {
  rootUrl?: string;
}
