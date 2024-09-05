/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiCommentsGetCommentsGet$Json } from '../fn/comments/api-comments-get-comments-get-json';
import { ApiCommentsGetCommentsGet$Json$Params } from '../fn/comments/api-comments-get-comments-get-json';
import { apiCommentsGetCommentsGet$Plain } from '../fn/comments/api-comments-get-comments-get-plain';
import { ApiCommentsGetCommentsGet$Plain$Params } from '../fn/comments/api-comments-get-comments-get-plain';
import { Callback } from '../models/callback';

@Injectable({ providedIn: 'root' })
export class CommentsService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiCommentsGetCommentsGet()` */
  static readonly ApiCommentsGetCommentsGetPath = '/api/Comments/getComments';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCommentsGetCommentsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCommentsGetCommentsGet$Plain$Response(params?: ApiCommentsGetCommentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Callback>>> {
    return apiCommentsGetCommentsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCommentsGetCommentsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCommentsGetCommentsGet$Plain(params?: ApiCommentsGetCommentsGet$Plain$Params, context?: HttpContext): Observable<Array<Callback>> {
    return this.apiCommentsGetCommentsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Callback>>): Array<Callback> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCommentsGetCommentsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCommentsGetCommentsGet$Json$Response(params?: ApiCommentsGetCommentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Callback>>> {
    return apiCommentsGetCommentsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCommentsGetCommentsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCommentsGetCommentsGet$Json(params?: ApiCommentsGetCommentsGet$Json$Params, context?: HttpContext): Observable<Array<Callback>> {
    return this.apiCommentsGetCommentsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Callback>>): Array<Callback> => r.body)
    );
  }

}
