/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiUserGet } from '../fn/user/api-user-get';
import { ApiUserGet$Params } from '../fn/user/api-user-get';

@Injectable({ providedIn: 'root' })
export class UserService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiUserGet()` */
  static readonly ApiUserGetPath = '/api/User';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGet$Response(params?: ApiUserGet$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiUserGet(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGet(params?: ApiUserGet$Params, context?: HttpContext): Observable<void> {
    return this.apiUserGet$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

}
