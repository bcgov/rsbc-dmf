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

import { UserProfile } from '../models/user-profile';

@Injectable({
  providedIn: 'root',
})
export class ProfileService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation apiProfileCurrentGet
   */
  static readonly ApiProfileCurrentGetPath = '/api/Profile/current';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileCurrentGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Plain$Response(params?: {
  }): Observable<StrictHttpResponse<UserProfile>> {

    const rb = new RequestBuilder(this.rootUrl, ProfileService.ApiProfileCurrentGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<UserProfile>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiProfileCurrentGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Plain(params?: {
  }): Observable<UserProfile> {

    return this.apiProfileCurrentGet$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<UserProfile>) => r.body as UserProfile)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileCurrentGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Json$Response(params?: {
  }): Observable<StrictHttpResponse<UserProfile>> {

    const rb = new RequestBuilder(this.rootUrl, ProfileService.ApiProfileCurrentGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<UserProfile>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiProfileCurrentGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Json(params?: {
  }): Observable<UserProfile> {

    return this.apiProfileCurrentGet$Json$Response(params).pipe(
      map((r: StrictHttpResponse<UserProfile>) => r.body as UserProfile)
    );
  }

}
