/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiUserAccessCreatePost$Json } from '../fn/user-access/api-user-access-create-post-json';
import { ApiUserAccessCreatePost$Json$Params } from '../fn/user-access/api-user-access-create-post-json';
import { apiUserAccessCreatePost$Plain } from '../fn/user-access/api-user-access-create-post-plain';
import { ApiUserAccessCreatePost$Plain$Params } from '../fn/user-access/api-user-access-create-post-plain';
import { OkResult } from '../models/ok-result';

@Injectable({ providedIn: 'root' })
export class UserAccessService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiUserAccessCreatePost()` */
  static readonly ApiUserAccessCreatePostPath = '/api/UserAccess/create';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserAccessCreatePost$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserAccessCreatePost$Plain$Response(params?: ApiUserAccessCreatePost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
    return apiUserAccessCreatePost$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserAccessCreatePost$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserAccessCreatePost$Plain(params?: ApiUserAccessCreatePost$Plain$Params, context?: HttpContext): Observable<OkResult> {
    return this.apiUserAccessCreatePost$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<OkResult>): OkResult => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserAccessCreatePost$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserAccessCreatePost$Json$Response(params?: ApiUserAccessCreatePost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
    return apiUserAccessCreatePost$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserAccessCreatePost$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserAccessCreatePost$Json(params?: ApiUserAccessCreatePost$Json$Params, context?: HttpContext): Observable<OkResult> {
    return this.apiUserAccessCreatePost$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<OkResult>): OkResult => r.body)
    );
  }

}
