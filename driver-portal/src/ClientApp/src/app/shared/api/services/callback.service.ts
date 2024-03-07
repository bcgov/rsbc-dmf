/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiCallbackDriverGet$Json } from '../fn/callback/api-callback-driver-get-json';
import { ApiCallbackDriverGet$Json$Params } from '../fn/callback/api-callback-driver-get-json';
import { apiCallbackDriverGet$Plain } from '../fn/callback/api-callback-driver-get-plain';
import { ApiCallbackDriverGet$Plain$Params } from '../fn/callback/api-callback-driver-get-plain';
import { Callback } from '../models/callback';

@Injectable({ providedIn: 'root' })
export class CallbackService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiCallbackDriverGet()` */
  static readonly ApiCallbackDriverGetPath = '/api/Callback/driver';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCallbackDriverGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCallbackDriverGet$Plain$Response(params?: ApiCallbackDriverGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Callback>>> {
    return apiCallbackDriverGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCallbackDriverGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCallbackDriverGet$Plain(params?: ApiCallbackDriverGet$Plain$Params, context?: HttpContext): Observable<Array<Callback>> {
    return this.apiCallbackDriverGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Callback>>): Array<Callback> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCallbackDriverGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCallbackDriverGet$Json$Response(params?: ApiCallbackDriverGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Callback>>> {
    return apiCallbackDriverGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCallbackDriverGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCallbackDriverGet$Json(params?: ApiCallbackDriverGet$Json$Params, context?: HttpContext): Observable<Array<Callback>> {
    return this.apiCallbackDriverGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Callback>>): Array<Callback> => r.body)
    );
  }

}
