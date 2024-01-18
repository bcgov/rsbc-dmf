/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDriverDriverIdAllDocumentsGet$Json } from '../fn/driver/api-driver-driver-id-all-documents-get-json';
import { ApiDriverDriverIdAllDocumentsGet$Json$Params } from '../fn/driver/api-driver-driver-id-all-documents-get-json';
import { apiDriverDriverIdAllDocumentsGet$Plain } from '../fn/driver/api-driver-driver-id-all-documents-get-plain';
import { ApiDriverDriverIdAllDocumentsGet$Plain$Params } from '../fn/driver/api-driver-driver-id-all-documents-get-plain';
import { apiDriverDriverIdDocumentsGet$Json } from '../fn/driver/api-driver-driver-id-documents-get-json';
import { ApiDriverDriverIdDocumentsGet$Json$Params } from '../fn/driver/api-driver-driver-id-documents-get-json';
import { apiDriverDriverIdDocumentsGet$Plain } from '../fn/driver/api-driver-driver-id-documents-get-plain';
import { ApiDriverDriverIdDocumentsGet$Plain$Params } from '../fn/driver/api-driver-driver-id-documents-get-plain';
import { CaseDocuments } from '../models/case-documents';
import { Document } from '../models/document';

@Injectable({ providedIn: 'root' })
export class DriverService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiDriverDriverIdDocumentsGet()` */
  static readonly ApiDriverDriverIdDocumentsGetPath = '/api/Driver/{driverId}/Documents';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverDriverIdDocumentsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverIdDocumentsGet$Plain$Response(params: ApiDriverDriverIdDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
    return apiDriverDriverIdDocumentsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverDriverIdDocumentsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverIdDocumentsGet$Plain(params: ApiDriverDriverIdDocumentsGet$Plain$Params, context?: HttpContext): Observable<CaseDocuments> {
    return this.apiDriverDriverIdDocumentsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocuments>): CaseDocuments => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverDriverIdDocumentsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverIdDocumentsGet$Json$Response(params: ApiDriverDriverIdDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
    return apiDriverDriverIdDocumentsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverDriverIdDocumentsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverIdDocumentsGet$Json(params: ApiDriverDriverIdDocumentsGet$Json$Params, context?: HttpContext): Observable<CaseDocuments> {
    return this.apiDriverDriverIdDocumentsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocuments>): CaseDocuments => r.body)
    );
  }

  /** Path part for operation `apiDriverDriverIdAllDocumentsGet()` */
  static readonly ApiDriverDriverIdAllDocumentsGetPath = '/api/Driver/{driverId}/AllDocuments';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverDriverIdAllDocumentsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverIdAllDocumentsGet$Plain$Response(params: ApiDriverDriverIdAllDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
    return apiDriverDriverIdAllDocumentsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverDriverIdAllDocumentsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverIdAllDocumentsGet$Plain(params: ApiDriverDriverIdAllDocumentsGet$Plain$Params, context?: HttpContext): Observable<Array<Document>> {
    return this.apiDriverDriverIdAllDocumentsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Document>>): Array<Document> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverDriverIdAllDocumentsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverIdAllDocumentsGet$Json$Response(params: ApiDriverDriverIdAllDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
    return apiDriverDriverIdAllDocumentsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverDriverIdAllDocumentsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverIdAllDocumentsGet$Json(params: ApiDriverDriverIdAllDocumentsGet$Json$Params, context?: HttpContext): Observable<Array<Document>> {
    return this.apiDriverDriverIdAllDocumentsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Document>>): Array<Document> => r.body)
    );
  }

}
