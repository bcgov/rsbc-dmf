/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiCommentsGet$Json } from '../fn/comments/api-comments-get-json';
import { ApiCommentsGet$Json$Params } from '../fn/comments/api-comments-get-json';
import { apiCommentsGet$Plain } from '../fn/comments/api-comments-get-plain';
import { ApiCommentsGet$Plain$Params } from '../fn/comments/api-comments-get-plain';
import { Callback } from '../models/callback';

@Injectable({ providedIn: 'root' })
export class CommentsService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiCommentsGet()` */
  static readonly ApiCommentsGetPath = '/api/Comments';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCommentsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCommentsGet$Plain$Response(params?: ApiCommentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Callback>>> {
    return apiCommentsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCommentsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCommentsGet$Plain(params?: ApiCommentsGet$Plain$Params, context?: HttpContext): Observable<Array<Callback>> {
    return this.apiCommentsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Callback>>): Array<Callback> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCommentsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCommentsGet$Json$Response(params?: ApiCommentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Callback>>> {
    return apiCommentsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCommentsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCommentsGet$Json(params?: ApiCommentsGet$Json$Params, context?: HttpContext): Observable<Array<Callback>> {
    return this.apiCommentsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Callback>>): Array<Callback> => r.body)
    );
  }

}
