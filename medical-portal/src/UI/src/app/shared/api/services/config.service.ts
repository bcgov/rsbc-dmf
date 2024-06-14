/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiConfigGet } from '../fn/config/api-config-get';
import { ApiConfigGet$Params } from '../fn/config/api-config-get';

@Injectable({ providedIn: 'root' })
export class ConfigService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiConfigGet()` */
  static readonly ApiConfigGetPath = '/api/Config';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiConfigGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Response(params?: ApiConfigGet$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiConfigGet(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiConfigGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet(params?: ApiConfigGet$Params, context?: HttpContext): Observable<void> {
    return this.apiConfigGet$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

}
