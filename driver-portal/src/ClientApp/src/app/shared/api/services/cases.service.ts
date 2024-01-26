/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiCasesClosedGet$Json } from '../fn/cases/api-cases-closed-get-json';
import { ApiCasesClosedGet$Json$Params } from '../fn/cases/api-cases-closed-get-json';
import { apiCasesClosedGet$Plain } from '../fn/cases/api-cases-closed-get-plain';
import { ApiCasesClosedGet$Plain$Params } from '../fn/cases/api-cases-closed-get-plain';
import { apiCasesMostRecentGet$Json } from '../fn/cases/api-cases-most-recent-get-json';
import { ApiCasesMostRecentGet$Json$Params } from '../fn/cases/api-cases-most-recent-get-json';
import { apiCasesMostRecentGet$Plain } from '../fn/cases/api-cases-most-recent-get-plain';
import { ApiCasesMostRecentGet$Plain$Params } from '../fn/cases/api-cases-most-recent-get-plain';
import { CaseDetail } from '../models/case-detail';

@Injectable({ providedIn: 'root' })
export class CasesService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiCasesClosedGet()` */
  static readonly ApiCasesClosedGetPath = '/api/Cases/Closed';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesClosedGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesClosedGet$Plain$Response(params?: ApiCasesClosedGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDetail>>> {
    return apiCasesClosedGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesClosedGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesClosedGet$Plain(params?: ApiCasesClosedGet$Plain$Params, context?: HttpContext): Observable<Array<CaseDetail>> {
    return this.apiCasesClosedGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<CaseDetail>>): Array<CaseDetail> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesClosedGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesClosedGet$Json$Response(params?: ApiCasesClosedGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDetail>>> {
    return apiCasesClosedGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesClosedGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesClosedGet$Json(params?: ApiCasesClosedGet$Json$Params, context?: HttpContext): Observable<Array<CaseDetail>> {
    return this.apiCasesClosedGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<CaseDetail>>): Array<CaseDetail> => r.body)
    );
  }

  /** Path part for operation `apiCasesMostRecentGet()` */
  static readonly ApiCasesMostRecentGetPath = '/api/Cases/MostRecent';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesMostRecentGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesMostRecentGet$Plain$Response(params?: ApiCasesMostRecentGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDetail>> {
    return apiCasesMostRecentGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesMostRecentGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesMostRecentGet$Plain(params?: ApiCasesMostRecentGet$Plain$Params, context?: HttpContext): Observable<CaseDetail> {
    return this.apiCasesMostRecentGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDetail>): CaseDetail => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesMostRecentGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesMostRecentGet$Json$Response(params?: ApiCasesMostRecentGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDetail>> {
    return apiCasesMostRecentGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesMostRecentGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesMostRecentGet$Json(params?: ApiCasesMostRecentGet$Json$Params, context?: HttpContext): Observable<CaseDetail> {
    return this.apiCasesMostRecentGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDetail>): CaseDetail => r.body)
    );
  }

}
