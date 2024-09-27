/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiConfigGet$Json } from '../fn/config/api-config-get-json';
import { ApiConfigGet$Json$Params } from '../fn/config/api-config-get-json';
import { apiConfigGet$Plain } from '../fn/config/api-config-get-plain';
import { ApiConfigGet$Plain$Params } from '../fn/config/api-config-get-plain';
import { PublicConfiguration } from '../models/public-configuration';

@Injectable({ providedIn: 'root' })
export class ConfigService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiConfigGet()` */
  static readonly ApiConfigGetPath = '/api/Config';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiConfigGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Plain$Response(params?: ApiConfigGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<PublicConfiguration>> {
    return apiConfigGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiConfigGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Plain(params?: ApiConfigGet$Plain$Params, context?: HttpContext): Observable<PublicConfiguration> {
    return this.apiConfigGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<PublicConfiguration>): PublicConfiguration => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiConfigGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Json$Response(params?: ApiConfigGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<PublicConfiguration>> {
    return apiConfigGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiConfigGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Json(params?: ApiConfigGet$Json$Params, context?: HttpContext): Observable<PublicConfiguration> {
    return this.apiConfigGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<PublicConfiguration>): PublicConfiguration => r.body)
    );
  }

}
