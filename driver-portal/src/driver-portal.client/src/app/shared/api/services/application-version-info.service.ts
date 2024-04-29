/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiApplicationVersionInfoGet$Json } from '../fn/application-version-info/api-application-version-info-get-json';
import { ApiApplicationVersionInfoGet$Json$Params } from '../fn/application-version-info/api-application-version-info-get-json';
import { apiApplicationVersionInfoGet$Plain } from '../fn/application-version-info/api-application-version-info-get-plain';
import { ApiApplicationVersionInfoGet$Plain$Params } from '../fn/application-version-info/api-application-version-info-get-plain';
import { ApplicationVersionInfo } from '../models/application-version-info';

@Injectable({ providedIn: 'root' })
export class ApplicationVersionInfoService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiApplicationVersionInfoGet()` */
  static readonly ApiApplicationVersionInfoGetPath = '/api/ApplicationVersionInfo';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiApplicationVersionInfoGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiApplicationVersionInfoGet$Plain$Response(params?: ApiApplicationVersionInfoGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<ApplicationVersionInfo>> {
    return apiApplicationVersionInfoGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiApplicationVersionInfoGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiApplicationVersionInfoGet$Plain(params?: ApiApplicationVersionInfoGet$Plain$Params, context?: HttpContext): Observable<ApplicationVersionInfo> {
    return this.apiApplicationVersionInfoGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<ApplicationVersionInfo>): ApplicationVersionInfo => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiApplicationVersionInfoGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiApplicationVersionInfoGet$Json$Response(params?: ApiApplicationVersionInfoGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<ApplicationVersionInfo>> {
    return apiApplicationVersionInfoGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiApplicationVersionInfoGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiApplicationVersionInfoGet$Json(params?: ApiApplicationVersionInfoGet$Json$Params, context?: HttpContext): Observable<ApplicationVersionInfo> {
    return this.apiApplicationVersionInfoGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<ApplicationVersionInfo>): ApplicationVersionInfo => r.body)
    );
  }

}
