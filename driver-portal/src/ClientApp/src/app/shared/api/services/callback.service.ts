/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiCallbackCancelPut$Json } from '../fn/callback/api-callback-cancel-put-json';
import { ApiCallbackCancelPut$Json$Params } from '../fn/callback/api-callback-cancel-put-json';
import { apiCallbackCancelPut$Plain } from '../fn/callback/api-callback-cancel-put-plain';
import { ApiCallbackCancelPut$Plain$Params } from '../fn/callback/api-callback-cancel-put-plain';
import { apiCallbackCreatePost$Json } from '../fn/callback/api-callback-create-post-json';
import { ApiCallbackCreatePost$Json$Params } from '../fn/callback/api-callback-create-post-json';
import { apiCallbackCreatePost$Plain } from '../fn/callback/api-callback-create-post-plain';
import { ApiCallbackCreatePost$Plain$Params } from '../fn/callback/api-callback-create-post-plain';
import { OkResult } from '../models/ok-result';

@Injectable({ providedIn: 'root' })
export class CallbackService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiCallbackCreatePost()` */
  static readonly ApiCallbackCreatePostPath = '/api/Callback/create';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCallbackCreatePost$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiCallbackCreatePost$Plain$Response(params?: ApiCallbackCreatePost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
    return apiCallbackCreatePost$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCallbackCreatePost$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiCallbackCreatePost$Plain(params?: ApiCallbackCreatePost$Plain$Params, context?: HttpContext): Observable<OkResult> {
    return this.apiCallbackCreatePost$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<OkResult>): OkResult => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCallbackCreatePost$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiCallbackCreatePost$Json$Response(params?: ApiCallbackCreatePost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
    return apiCallbackCreatePost$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCallbackCreatePost$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiCallbackCreatePost$Json(params?: ApiCallbackCreatePost$Json$Params, context?: HttpContext): Observable<OkResult> {
    return this.apiCallbackCreatePost$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<OkResult>): OkResult => r.body)
    );
  }

  /** Path part for operation `apiCallbackCancelPut()` */
  static readonly ApiCallbackCancelPutPath = '/api/Callback/cancel';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCallbackCancelPut$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiCallbackCancelPut$Plain$Response(params?: ApiCallbackCancelPut$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
    return apiCallbackCancelPut$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCallbackCancelPut$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiCallbackCancelPut$Plain(params?: ApiCallbackCancelPut$Plain$Params, context?: HttpContext): Observable<OkResult> {
    return this.apiCallbackCancelPut$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<OkResult>): OkResult => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCallbackCancelPut$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiCallbackCancelPut$Json$Response(params?: ApiCallbackCancelPut$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
    return apiCallbackCancelPut$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCallbackCancelPut$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiCallbackCancelPut$Json(params?: ApiCallbackCancelPut$Json$Params, context?: HttpContext): Observable<OkResult> {
    return this.apiCallbackCancelPut$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<OkResult>): OkResult => r.body)
    );
  }

}
