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
import { apiChefsSubmissionGet$Json } from '../fn/chefs/api-chefs-submission-get-json';
import { ApiChefsSubmissionGet$Json$Params } from '../fn/chefs/api-chefs-submission-get-json';
import { apiChefsSubmissionGet$Plain } from '../fn/chefs/api-chefs-submission-get-plain';
import { ApiChefsSubmissionGet$Plain$Params } from '../fn/chefs/api-chefs-submission-get-plain';
import { apiChefsSubmissionPut$Json } from '../fn/chefs/api-chefs-submission-put-json';
import { ApiChefsSubmissionPut$Json$Params } from '../fn/chefs/api-chefs-submission-put-json';
import { apiChefsSubmissionPut$Plain } from '../fn/chefs/api-chefs-submission-put-plain';
import { ApiChefsSubmissionPut$Plain$Params } from '../fn/chefs/api-chefs-submission-put-plain';
import { ChefsBundle } from '../models/chefs-bundle';
import { ChefsSubmission } from '../models/chefs-submission';

@Injectable({ providedIn: 'root' })
export class ChefsService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiChefsSubmissionGet()` */
  static readonly ApiChefsSubmissionGetPath = '/api/Chefs/submission';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsSubmissionGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsSubmissionGet$Plain$Response(params?: ApiChefsSubmissionGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsSubmission>> {
    return apiChefsSubmissionGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsSubmissionGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsSubmissionGet$Plain(params?: ApiChefsSubmissionGet$Plain$Params, context?: HttpContext): Observable<ChefsSubmission> {
    return this.apiChefsSubmissionGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<ChefsSubmission>): ChefsSubmission => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsSubmissionGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsSubmissionGet$Json$Response(params?: ApiChefsSubmissionGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsSubmission>> {
    return apiChefsSubmissionGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsSubmissionGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsSubmissionGet$Json(params?: ApiChefsSubmissionGet$Json$Params, context?: HttpContext): Observable<ChefsSubmission> {
    return this.apiChefsSubmissionGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<ChefsSubmission>): ChefsSubmission => r.body)
    );
  }

  /** Path part for operation `apiChefsSubmissionPut()` */
  static readonly ApiChefsSubmissionPutPath = '/api/Chefs/submission';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsSubmissionPut$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiChefsSubmissionPut$Plain$Response(params?: ApiChefsSubmissionPut$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsSubmission>> {
    return apiChefsSubmissionPut$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsSubmissionPut$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiChefsSubmissionPut$Plain(params?: ApiChefsSubmissionPut$Plain$Params, context?: HttpContext): Observable<ChefsSubmission> {
    return this.apiChefsSubmissionPut$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<ChefsSubmission>): ChefsSubmission => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsSubmissionPut$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiChefsSubmissionPut$Json$Response(params?: ApiChefsSubmissionPut$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsSubmission>> {
    return apiChefsSubmissionPut$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsSubmissionPut$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiChefsSubmissionPut$Json(params?: ApiChefsSubmissionPut$Json$Params, context?: HttpContext): Observable<ChefsSubmission> {
    return this.apiChefsSubmissionPut$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<ChefsSubmission>): ChefsSubmission => r.body)
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
  apiChefsBundleGet$Plain$Response(params: ApiChefsBundleGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsBundle>> {
    return apiChefsBundleGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsBundleGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsBundleGet$Plain(params: ApiChefsBundleGet$Plain$Params, context?: HttpContext): Observable<ChefsBundle> {
    return this.apiChefsBundleGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<ChefsBundle>): ChefsBundle => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsBundleGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsBundleGet$Json$Response(params: ApiChefsBundleGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsBundle>> {
    return apiChefsBundleGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsBundleGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiChefsBundleGet$Json(params: ApiChefsBundleGet$Json$Params, context?: HttpContext): Observable<ChefsBundle> {
    return this.apiChefsBundleGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<ChefsBundle>): ChefsBundle => r.body)
    );
  }

}
