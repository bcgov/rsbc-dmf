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
import { apiDriverDriverSessionGet$Json } from '../fn/driver/api-driver-driver-session-get-json';
import { ApiDriverDriverSessionGet$Json$Params } from '../fn/driver/api-driver-driver-session-get-json';
import { apiDriverDriverSessionGet$Plain } from '../fn/driver/api-driver-driver-session-get-plain';
import { ApiDriverDriverSessionGet$Plain$Params } from '../fn/driver/api-driver-driver-session-get-plain';
import { apiDriverInfoDriverLicenceNumberSurCodeGet$Json } from '../fn/driver/api-driver-info-driver-licence-number-sur-code-get-json';
import { ApiDriverInfoDriverLicenceNumberSurCodeGet$Json$Params } from '../fn/driver/api-driver-info-driver-licence-number-sur-code-get-json';
import { apiDriverInfoDriverLicenceNumberSurCodeGet$Plain } from '../fn/driver/api-driver-info-driver-licence-number-sur-code-get-plain';
import { ApiDriverInfoDriverLicenceNumberSurCodeGet$Plain$Params } from '../fn/driver/api-driver-info-driver-licence-number-sur-code-get-plain';
import { Document } from '../models/document';
import { Driver } from '../models/driver';
import { UserContext } from '../models/user-context';

@Injectable({ providedIn: 'root' })
export class DriverService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
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

  /** Path part for operation `apiDriverInfoDriverLicenceNumberSurCodeGet()` */
  static readonly ApiDriverInfoDriverLicenceNumberSurCodeGetPath = '/api/Driver/info/{driverLicenceNumber}/{surCode}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverInfoDriverLicenceNumberSurCodeGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverInfoDriverLicenceNumberSurCodeGet$Plain$Response(params: ApiDriverInfoDriverLicenceNumberSurCodeGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Driver>> {
    return apiDriverInfoDriverLicenceNumberSurCodeGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverInfoDriverLicenceNumberSurCodeGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverInfoDriverLicenceNumberSurCodeGet$Plain(params: ApiDriverInfoDriverLicenceNumberSurCodeGet$Plain$Params, context?: HttpContext): Observable<Driver> {
    return this.apiDriverInfoDriverLicenceNumberSurCodeGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Driver>): Driver => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverInfoDriverLicenceNumberSurCodeGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverInfoDriverLicenceNumberSurCodeGet$Json$Response(params: ApiDriverInfoDriverLicenceNumberSurCodeGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Driver>> {
    return apiDriverInfoDriverLicenceNumberSurCodeGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverInfoDriverLicenceNumberSurCodeGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverInfoDriverLicenceNumberSurCodeGet$Json(params: ApiDriverInfoDriverLicenceNumberSurCodeGet$Json$Params, context?: HttpContext): Observable<Driver> {
    return this.apiDriverInfoDriverLicenceNumberSurCodeGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Driver>): Driver => r.body)
    );
  }

  /** Path part for operation `apiDriverDriverSessionGet()` */
  static readonly ApiDriverDriverSessionGetPath = '/api/Driver/driverSession';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverDriverSessionGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverSessionGet$Plain$Response(params?: ApiDriverDriverSessionGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<UserContext>> {
    return apiDriverDriverSessionGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverDriverSessionGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverSessionGet$Plain(params?: ApiDriverDriverSessionGet$Plain$Params, context?: HttpContext): Observable<UserContext> {
    return this.apiDriverDriverSessionGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<UserContext>): UserContext => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverDriverSessionGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverSessionGet$Json$Response(params?: ApiDriverDriverSessionGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<UserContext>> {
    return apiDriverDriverSessionGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverDriverSessionGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverDriverSessionGet$Json(params?: ApiDriverDriverSessionGet$Json$Params, context?: HttpContext): Observable<UserContext> {
    return this.apiDriverDriverSessionGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<UserContext>): UserContext => r.body)
    );
  }

}
