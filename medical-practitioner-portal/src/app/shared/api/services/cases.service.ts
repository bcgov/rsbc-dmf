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
import { DmerCaseListItem } from '../models/dmer-case-list-item';
import { PatientCase } from '../models/patient-case';

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
  apiCasesCaseIdGet$Plain$Response(params: ApiCasesCaseIdGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<PatientCase>> {
    return apiCasesCaseIdGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesCaseIdGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesCaseIdGet$Plain(params: ApiCasesCaseIdGet$Plain$Params, context?: HttpContext): Observable<PatientCase> {
    return this.apiCasesCaseIdGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<PatientCase>): PatientCase => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesCaseIdGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesCaseIdGet$Json$Response(params: ApiCasesCaseIdGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<PatientCase>> {
    return apiCasesCaseIdGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesCaseIdGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesCaseIdGet$Json(params: ApiCasesCaseIdGet$Json$Params, context?: HttpContext): Observable<PatientCase> {
    return this.apiCasesCaseIdGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<PatientCase>): PatientCase => r.body)
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
  apiCasesGet$Plain$Response(params?: ApiCasesGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DmerCaseListItem>>> {
    return apiCasesGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Plain(params?: ApiCasesGet$Plain$Params, context?: HttpContext): Observable<Array<DmerCaseListItem>> {
    return this.apiCasesGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<DmerCaseListItem>>): Array<DmerCaseListItem> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Json$Response(params?: ApiCasesGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DmerCaseListItem>>> {
    return apiCasesGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiCasesGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Json(params?: ApiCasesGet$Json$Params, context?: HttpContext): Observable<Array<DmerCaseListItem>> {
    return this.apiCasesGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<DmerCaseListItem>>): Array<DmerCaseListItem> => r.body)
    );
  }

}
