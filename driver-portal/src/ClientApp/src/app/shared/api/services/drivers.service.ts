/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDriversDriverIdDocumentsGet$Json } from '../fn/drivers/api-drivers-driver-id-documents-get-json';
import { ApiDriversDriverIdDocumentsGet$Json$Params } from '../fn/drivers/api-drivers-driver-id-documents-get-json';
import { apiDriversDriverIdDocumentsGet$Plain } from '../fn/drivers/api-drivers-driver-id-documents-get-plain';
import { ApiDriversDriverIdDocumentsGet$Plain$Params } from '../fn/drivers/api-drivers-driver-id-documents-get-plain';
import { CaseDocuments } from '../models/case-documents';

@Injectable({ providedIn: 'root' })
export class DriversService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiDriversDriverIdDocumentsGet()` */
  static readonly ApiDriversDriverIdDocumentsGetPath = '/api/Drivers/{driverId}/Documents';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriversDriverIdDocumentsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriversDriverIdDocumentsGet$Plain$Response(params: ApiDriversDriverIdDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
    return apiDriversDriverIdDocumentsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriversDriverIdDocumentsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriversDriverIdDocumentsGet$Plain(params: ApiDriversDriverIdDocumentsGet$Plain$Params, context?: HttpContext): Observable<CaseDocuments> {
    return this.apiDriversDriverIdDocumentsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocuments>): CaseDocuments => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriversDriverIdDocumentsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriversDriverIdDocumentsGet$Json$Response(params: ApiDriversDriverIdDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
    return apiDriversDriverIdDocumentsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriversDriverIdDocumentsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriversDriverIdDocumentsGet$Json(params: ApiDriversDriverIdDocumentsGet$Json$Params, context?: HttpContext): Observable<CaseDocuments> {
    return this.apiDriversDriverIdDocumentsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocuments>): CaseDocuments => r.body)
    );
  }

}
