/* tslint:disable */

/* eslint-disable */
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';

import { APP_CONFIG, AppConfig } from '@app/app.config';

import { ApiConfiguration } from '../api-configuration';
import { BaseService } from '../base-service';
import { Configuration } from '../models/configuration';
import { RequestBuilder } from '../request-builder';
import { StrictHttpResponse } from '../strict-http-response';

@Injectable({
  providedIn: 'root',
})
export class ConfigService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /**
   * Path part for operation apiConfigGet
   */
  static readonly ApiConfigGetPath = '/api/Config';

  /**
   * Get the client configuration for this environment.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiConfigGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Plain$Response(params?: {}): Observable<
    StrictHttpResponse<Configuration>
  > {
    const rb = new RequestBuilder(
      this.rootUrl,
      ConfigService.ApiConfigGetPath,
      'get'
    );
    if (params) {
    }

    return this.http
      .request(
        rb.build({
          responseType: 'text',
          accept: 'text/plain',
        })
      )
      .pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
          return r as StrictHttpResponse<Configuration>;
        })
      );
  }

  /**
   * Get the client configuration for this environment.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiConfigGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Plain(params?: {}): Observable<Configuration> {
    return this.apiConfigGet$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<Configuration>) => r.body as Configuration)
    );
  }

  /**
   * Get the client configuration for this environment.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiConfigGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Json$Response(params?: {}): Observable<
    StrictHttpResponse<Configuration>
  > {
    const rb = new RequestBuilder(
      this.rootUrl,
      ConfigService.ApiConfigGetPath,
      'get'
    );
    if (params) {
    }

    return this.http
      .request(
        rb.build({
          responseType: 'json',
          accept: 'text/json',
        })
      )
      .pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
          return r as StrictHttpResponse<Configuration>;
        })
      );
  }

  /**
   * Get the client configuration for this environment.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiConfigGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiConfigGet$Json(params?: {}): Observable<Configuration> {
    return this.apiConfigGet$Json$Response(params).pipe(
      map((r: StrictHttpResponse<Configuration>) => r.body as Configuration)
    );
  }
}
