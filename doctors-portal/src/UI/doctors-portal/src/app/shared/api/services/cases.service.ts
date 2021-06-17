/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { Case } from '../models/case';

@Injectable({
  providedIn: 'root',
})
export class CasesService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation apiCasesGet
   */
  static readonly ApiCasesGetPath = '/api/Cases';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Plain$Response(params?: {
    ByCaseId?: string;
    ByPatientName?: string;
    ByDriverLicense?: string;
    ByStatus?: Array<string>;
  }): Observable<StrictHttpResponse<Array<Case>>> {

    const rb = new RequestBuilder(this.rootUrl, CasesService.ApiCasesGetPath, 'get');
    if (params) {
      rb.query('ByCaseId', params.ByCaseId, {});
      rb.query('ByPatientName', params.ByPatientName, {});
      rb.query('ByDriverLicense', params.ByDriverLicense, {});
      rb.query('ByStatus', params.ByStatus, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<Case>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiCasesGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Plain(params?: {
    ByCaseId?: string;
    ByPatientName?: string;
    ByDriverLicense?: string;
    ByStatus?: Array<string>;
  }): Observable<Array<Case>> {

    return this.apiCasesGet$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<Array<Case>>) => r.body as Array<Case>)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiCasesGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Json$Response(params?: {
    ByCaseId?: string;
    ByPatientName?: string;
    ByDriverLicense?: string;
    ByStatus?: Array<string>;
  }): Observable<StrictHttpResponse<Array<Case>>> {

    const rb = new RequestBuilder(this.rootUrl, CasesService.ApiCasesGetPath, 'get');
    if (params) {
      rb.query('ByCaseId', params.ByCaseId, {});
      rb.query('ByPatientName', params.ByPatientName, {});
      rb.query('ByDriverLicense', params.ByDriverLicense, {});
      rb.query('ByStatus', params.ByStatus, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<Case>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiCasesGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiCasesGet$Json(params?: {
    ByCaseId?: string;
    ByPatientName?: string;
    ByDriverLicense?: string;
    ByStatus?: Array<string>;
  }): Observable<Array<Case>> {

    return this.apiCasesGet$Json$Response(params).pipe(
      map((r: StrictHttpResponse<Array<Case>>) => r.body as Array<Case>)
    );
  }

}
