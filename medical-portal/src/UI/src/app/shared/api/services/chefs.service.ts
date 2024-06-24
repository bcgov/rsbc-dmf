/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiChefsBundleGet$Json } from '../fn/chefs/api-chefs-bundle-get-json';
import { ApiChefsBundleGet$Json$Params } from '../fn/chefs/api-chefs-bundle-get-json';
import { apiChefsBundleGet$Plain } from '../fn/chefs/api-chefs-bundle-get-plain';
import { ApiChefsBundleGet$Plain$Params } from '../fn/chefs/api-chefs-bundle-get-plain';
import { apiChefsSubmissionGet } from '../fn/chefs/api-chefs-submission-get';
import { ApiChefsSubmissionGet$Params } from '../fn/chefs/api-chefs-submission-get';
import { apiChefsSubmissionPut } from '../fn/chefs/api-chefs-submission-put';
import { ApiChefsSubmissionPut$Params } from '../fn/chefs/api-chefs-submission-put';
import { Document } from '../models/document';

@Injectable({ providedIn: 'root' })
export class ChefsService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiChefsSubmissionGet()` */
  static readonly ApiChefsSubmissionGetPath = '/api/Chefs/submission';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsSubmissionGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsSubmissionGet$Response(params?: ApiChefsSubmissionGet$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiChefsSubmissionGet(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsSubmissionGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsSubmissionGet(params?: ApiChefsSubmissionGet$Params, context?: HttpContext): Observable<void> {
    return this.apiChefsSubmissionGet$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `apiChefsSubmissionPut()` */
  static readonly ApiChefsSubmissionPutPath = '/api/Chefs/submission';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsSubmissionPut()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiChefsSubmissionPut$Response(params?: ApiChefsSubmissionPut$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiChefsSubmissionPut(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsSubmissionPut$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiChefsSubmissionPut(params?: ApiChefsSubmissionPut$Params, context?: HttpContext): Observable<void> {
    return this.apiChefsSubmissionPut$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `apiChefsBundleGet()` */
  static readonly ApiChefsBundleGetPath = '/api/Chefs/bundle';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsBundleGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsBundleGet$Plain$Response(params: ApiChefsBundleGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
    return apiChefsBundleGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsBundleGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsBundleGet$Plain(params: ApiChefsBundleGet$Plain$Params, context?: HttpContext): Observable<Array<Document>> {
    return this.apiChefsBundleGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Document>>): Array<Document> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsBundleGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsBundleGet$Json$Response(params: ApiChefsBundleGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
    return apiChefsBundleGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsBundleGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsBundleGet$Json(params: ApiChefsBundleGet$Json$Params, context?: HttpContext): Observable<Array<Document>> {
    return this.apiChefsBundleGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Document>>): Array<Document> => r.body)
    );
  }

}
