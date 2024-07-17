/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDriverInfoDriverLicenceNumberGet$Json } from '../fn/driver/api-driver-info-driver-licence-number-get-json';
import { ApiDriverInfoDriverLicenceNumberGet$Json$Params } from '../fn/driver/api-driver-info-driver-licence-number-get-json';
import { apiDriverInfoDriverLicenceNumberGet$Plain } from '../fn/driver/api-driver-info-driver-licence-number-get-plain';
import { ApiDriverInfoDriverLicenceNumberGet$Plain$Params } from '../fn/driver/api-driver-info-driver-licence-number-get-plain';
import { Driver } from '../models/driver';

@Injectable({ providedIn: 'root' })
export class DriverService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiDriverInfoDriverLicenceNumberGet()` */
  static readonly ApiDriverInfoDriverLicenceNumberGetPath = '/api/Driver/info/{driverLicenceNumber}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverInfoDriverLicenceNumberGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverInfoDriverLicenceNumberGet$Plain$Response(params: ApiDriverInfoDriverLicenceNumberGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Driver>> {
    return apiDriverInfoDriverLicenceNumberGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverInfoDriverLicenceNumberGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverInfoDriverLicenceNumberGet$Plain(params: ApiDriverInfoDriverLicenceNumberGet$Plain$Params, context?: HttpContext): Observable<Driver> {
    return this.apiDriverInfoDriverLicenceNumberGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Driver>): Driver => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDriverInfoDriverLicenceNumberGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverInfoDriverLicenceNumberGet$Json$Response(params: ApiDriverInfoDriverLicenceNumberGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Driver>> {
    return apiDriverInfoDriverLicenceNumberGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDriverInfoDriverLicenceNumberGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDriverInfoDriverLicenceNumberGet$Json(params: ApiDriverInfoDriverLicenceNumberGet$Json$Params, context?: HttpContext): Observable<Driver> {
    return this.apiDriverInfoDriverLicenceNumberGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Driver>): Driver => r.body)
    );
  }

}
