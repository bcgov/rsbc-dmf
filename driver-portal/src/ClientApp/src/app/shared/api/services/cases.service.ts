/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiCasesCaseIdGet$Json } from '../fn/cases/api-cases-case-id-get-json';
import { ApiCasesCaseIdGet$Json$Params } from '../fn/cases/api-cases-case-id-get-json';
import { apiCasesCaseIdGet$Plain } from '../fn/cases/api-cases-case-id-get-plain';
import { ApiCasesCaseIdGet$Plain$Params } from '../fn/cases/api-cases-case-id-get-plain';
import { apiCasesGet$Json } from '../fn/cases/api-cases-get-json';
import { ApiCasesGet$Json$Params } from '../fn/cases/api-cases-get-json';
import { apiCasesGet$Plain } from '../fn/cases/api-cases-get-plain';
import { ApiCasesGet$Plain$Params } from '../fn/cases/api-cases-get-plain';
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

  /** Path part for operation `apiCasesCaseIdGet()` */
  static readonly ApiCasesCaseIdGetPath = '/api/Cases/{caseId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesCaseIdGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesCaseIdGet$Plain$Response(params: ApiCasesCaseIdGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDetail>> {
    return apiCasesCaseIdGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesCaseIdGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesCaseIdGet$Plain(params: ApiCasesCaseIdGet$Plain$Params, context?: HttpContext): Observable<CaseDetail> {
    return this.apiCasesCaseIdGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDetail>): CaseDetail => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesCaseIdGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesCaseIdGet$Json$Response(params: ApiCasesCaseIdGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDetail>> {
    return apiCasesCaseIdGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesCaseIdGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesCaseIdGet$Json(params: ApiCasesCaseIdGet$Json$Params, context?: HttpContext): Observable<CaseDetail> {
    return this.apiCasesCaseIdGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDetail>): CaseDetail => r.body)
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

  /** Path part for operation `apiCasesGet()` */
  static readonly ApiCasesGetPath = '/api/Cases';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Plain$Response(params?: ApiCasesGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDetail>>> {
    return apiCasesGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Plain(params?: ApiCasesGet$Plain$Params, context?: HttpContext): Observable<Array<CaseDetail>> {
    return this.apiCasesGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<CaseDetail>>): Array<CaseDetail> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Json$Response(params?: ApiCasesGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDetail>>> {
    return apiCasesGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Json(params?: ApiCasesGet$Json$Params, context?: HttpContext): Observable<Array<CaseDetail>> {
    return this.apiCasesGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<CaseDetail>>): Array<CaseDetail> => r.body)
    );
  }

}
