/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiProfileCurrentGet$Json } from '../fn/profile/api-profile-current-get-json';
import { ApiProfileCurrentGet$Json$Params } from '../fn/profile/api-profile-current-get-json';
import { apiProfileCurrentGet$Plain } from '../fn/profile/api-profile-current-get-plain';
import { ApiProfileCurrentGet$Plain$Params } from '../fn/profile/api-profile-current-get-plain';
import { apiProfileRegisterPut$Json } from '../fn/profile/api-profile-register-put-json';
import { ApiProfileRegisterPut$Json$Params } from '../fn/profile/api-profile-register-put-json';
import { apiProfileRegisterPut$Plain } from '../fn/profile/api-profile-register-put-plain';
import { ApiProfileRegisterPut$Plain$Params } from '../fn/profile/api-profile-register-put-plain';
import { OkResult } from '../models/ok-result';
import { UserProfile } from '../models/user-profile';

@Injectable({ providedIn: 'root' })
export class ProfileService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiProfileCurrentGet()` */
  static readonly ApiProfileCurrentGetPath = '/api/Profile/current';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileCurrentGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Plain$Response(params?: ApiProfileCurrentGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<UserProfile>> {
    return apiProfileCurrentGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfileCurrentGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Plain(params?: ApiProfileCurrentGet$Plain$Params, context?: HttpContext): Observable<UserProfile> {
    return this.apiProfileCurrentGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<UserProfile>): UserProfile => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileCurrentGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Json$Response(params?: ApiProfileCurrentGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<UserProfile>> {
    return apiProfileCurrentGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfileCurrentGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Json(params?: ApiProfileCurrentGet$Json$Params, context?: HttpContext): Observable<UserProfile> {
    return this.apiProfileCurrentGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<UserProfile>): UserProfile => r.body)
    );
  }

  /** Path part for operation `apiProfileRegisterPut()` */
  static readonly ApiProfileRegisterPutPath = '/api/Profile/register';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileRegisterPut$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfileRegisterPut$Plain$Response(params?: ApiProfileRegisterPut$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
    return apiProfileRegisterPut$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfileRegisterPut$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfileRegisterPut$Plain(params?: ApiProfileRegisterPut$Plain$Params, context?: HttpContext): Observable<OkResult> {
    return this.apiProfileRegisterPut$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<OkResult>): OkResult => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileRegisterPut$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfileRegisterPut$Json$Response(params?: ApiProfileRegisterPut$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
    return apiProfileRegisterPut$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfileRegisterPut$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfileRegisterPut$Json(params?: ApiProfileRegisterPut$Json$Params, context?: HttpContext): Observable<OkResult> {
    return this.apiProfileRegisterPut$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<OkResult>): OkResult => r.body)
    );
  }

}
