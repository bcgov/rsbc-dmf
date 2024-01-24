/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDriverAllDocumentsGet$Json } from '../fn/driver/api-driver-all-documents-get-json';
import { ApiDriverAllDocumentsGet$Json$Params } from '../fn/driver/api-driver-all-documents-get-json';
import { apiDriverAllDocumentsGet$Plain } from '../fn/driver/api-driver-all-documents-get-plain';
import { ApiDriverAllDocumentsGet$Plain$Params } from '../fn/driver/api-driver-all-documents-get-plain';
import { apiDriverDocumentsGet$Json } from '../fn/driver/api-driver-documents-get-json';
import { ApiDriverDocumentsGet$Json$Params } from '../fn/driver/api-driver-documents-get-json';
import { apiDriverDocumentsGet$Plain } from '../fn/driver/api-driver-documents-get-plain';
import { ApiDriverDocumentsGet$Plain$Params } from '../fn/driver/api-driver-documents-get-plain';
import { CaseDocuments } from '../models/case-documents';
import { Document } from '../models/document';

@Injectable({ providedIn: 'root' })
export class DriverService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiDriverDocumentsGet()` */
  static readonly ApiDriverDocumentsGetPath = '/api/Driver/Documents';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverDocumentsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDocumentsGet$Plain$Response(params?: ApiDriverDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
    return apiDriverDocumentsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverDocumentsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDocumentsGet$Plain(params?: ApiDriverDocumentsGet$Plain$Params, context?: HttpContext): Observable<CaseDocuments> {
    return this.apiDriverDocumentsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocuments>): CaseDocuments => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverDocumentsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDocumentsGet$Json$Response(params?: ApiDriverDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
    return apiDriverDocumentsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverDocumentsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDocumentsGet$Json(params?: ApiDriverDocumentsGet$Json$Params, context?: HttpContext): Observable<CaseDocuments> {
    return this.apiDriverDocumentsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocuments>): CaseDocuments => r.body)
    );
  }

  /** Path part for operation `apiDriverAllDocumentsGet()` */
  static readonly ApiDriverAllDocumentsGetPath = '/api/Driver/AllDocuments';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverAllDocumentsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverAllDocumentsGet$Plain$Response(params?: ApiDriverAllDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
    return apiDriverAllDocumentsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverAllDocumentsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverAllDocumentsGet$Plain(params?: ApiDriverAllDocumentsGet$Plain$Params, context?: HttpContext): Observable<Array<Document>> {
    return this.apiDriverAllDocumentsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Document>>): Array<Document> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverAllDocumentsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverAllDocumentsGet$Json$Response(params?: ApiDriverAllDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
    return apiDriverAllDocumentsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverAllDocumentsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverAllDocumentsGet$Json(params?: ApiDriverAllDocumentsGet$Json$Params, context?: HttpContext): Observable<Array<Document>> {
    return this.apiDriverAllDocumentsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Document>>): Array<Document> => r.body)
    );
  }

}
