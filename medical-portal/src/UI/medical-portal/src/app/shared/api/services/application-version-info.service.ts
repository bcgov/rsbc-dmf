/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { ApplicationVersionInfo } from '../models/application-version-info';

@Injectable({
  providedIn: 'root',
})
export class ApplicationVersionInfoService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation apiApplicationVersionInfoGet
   */
  static readonly ApiApplicationVersionInfoGetPath = '/api/ApplicationVersionInfo';

  /**
   * Return the version of the running application.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiApplicationVersionInfoGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiApplicationVersionInfoGet$Plain$Response(params?: {
  }): Observable<StrictHttpResponse<ApplicationVersionInfo>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationVersionInfoService.ApiApplicationVersionInfoGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ApplicationVersionInfo>;
      })
    );
  }

  /**
   * Return the version of the running application.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiApplicationVersionInfoGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiApplicationVersionInfoGet$Plain(params?: {
  }): Observable<ApplicationVersionInfo> {

    return this.apiApplicationVersionInfoGet$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<ApplicationVersionInfo>) => r.body as ApplicationVersionInfo)
    );
  }

  /**
   * Return the version of the running application.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiApplicationVersionInfoGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiApplicationVersionInfoGet$Json$Response(params?: {
  }): Observable<StrictHttpResponse<ApplicationVersionInfo>> {

    const rb = new RequestBuilder(this.rootUrl, ApplicationVersionInfoService.ApiApplicationVersionInfoGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ApplicationVersionInfo>;
      })
    );
  }

  /**
   * Return the version of the running application.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiApplicationVersionInfoGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiApplicationVersionInfoGet$Json(params?: {
  }): Observable<ApplicationVersionInfo> {

    return this.apiApplicationVersionInfoGet$Json$Response(params).pipe(
      map((r: StrictHttpResponse<ApplicationVersionInfo>) => r.body as ApplicationVersionInfo)
    );
  }

}
