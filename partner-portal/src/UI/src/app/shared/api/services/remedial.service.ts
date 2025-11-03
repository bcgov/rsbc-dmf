/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiRemedialGetIgnitionInterlockGet$Json } from '../fn/remedial/api-remedial-get-ignition-interlock-get-json';
import { ApiRemedialGetIgnitionInterlockGet$Json$Params } from '../fn/remedial/api-remedial-get-ignition-interlock-get-json';
import { apiRemedialGetIgnitionInterlockGet$Plain } from '../fn/remedial/api-remedial-get-ignition-interlock-get-plain';
import { ApiRemedialGetIgnitionInterlockGet$Plain$Params } from '../fn/remedial/api-remedial-get-ignition-interlock-get-plain';
import { apiRemedialGetRehabTriggersGet$Json } from '../fn/remedial/api-remedial-get-rehab-triggers-get-json';
import { ApiRemedialGetRehabTriggersGet$Json$Params } from '../fn/remedial/api-remedial-get-rehab-triggers-get-json';
import { apiRemedialGetRehabTriggersGet$Plain } from '../fn/remedial/api-remedial-get-rehab-triggers-get-plain';
import { ApiRemedialGetRehabTriggersGet$Plain$Params } from '../fn/remedial/api-remedial-get-rehab-triggers-get-plain';
import { IgnitionInterlock } from '../models/ignition-interlock';
import { RehabTrigger } from '../models/rehab-trigger';

@Injectable({ providedIn: 'root' })
export class RemedialService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiRemedialGetIgnitionInterlockGet()` */
  static readonly ApiRemedialGetIgnitionInterlockGetPath = '/api/Remedial/getIgnitionInterlock';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiRemedialGetIgnitionInterlockGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiRemedialGetIgnitionInterlockGet$Plain$Response(params?: ApiRemedialGetIgnitionInterlockGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<IgnitionInterlock>>> {
    return apiRemedialGetIgnitionInterlockGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiRemedialGetIgnitionInterlockGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiRemedialGetIgnitionInterlockGet$Plain(params?: ApiRemedialGetIgnitionInterlockGet$Plain$Params, context?: HttpContext): Observable<Array<IgnitionInterlock>> {
    return this.apiRemedialGetIgnitionInterlockGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<IgnitionInterlock>>): Array<IgnitionInterlock> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiRemedialGetIgnitionInterlockGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiRemedialGetIgnitionInterlockGet$Json$Response(params?: ApiRemedialGetIgnitionInterlockGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<IgnitionInterlock>>> {
    return apiRemedialGetIgnitionInterlockGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiRemedialGetIgnitionInterlockGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiRemedialGetIgnitionInterlockGet$Json(params?: ApiRemedialGetIgnitionInterlockGet$Json$Params, context?: HttpContext): Observable<Array<IgnitionInterlock>> {
    return this.apiRemedialGetIgnitionInterlockGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<IgnitionInterlock>>): Array<IgnitionInterlock> => r.body)
    );
  }

  /** Path part for operation `apiRemedialGetRehabTriggersGet()` */
  static readonly ApiRemedialGetRehabTriggersGetPath = '/api/Remedial/getRehabTriggers';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiRemedialGetRehabTriggersGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiRemedialGetRehabTriggersGet$Plain$Response(params?: ApiRemedialGetRehabTriggersGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<RehabTrigger>>> {
    return apiRemedialGetRehabTriggersGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiRemedialGetRehabTriggersGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiRemedialGetRehabTriggersGet$Plain(params?: ApiRemedialGetRehabTriggersGet$Plain$Params, context?: HttpContext): Observable<Array<RehabTrigger>> {
    return this.apiRemedialGetRehabTriggersGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<RehabTrigger>>): Array<RehabTrigger> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiRemedialGetRehabTriggersGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiRemedialGetRehabTriggersGet$Json$Response(params?: ApiRemedialGetRehabTriggersGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<RehabTrigger>>> {
    return apiRemedialGetRehabTriggersGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiRemedialGetRehabTriggersGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiRemedialGetRehabTriggersGet$Json(params?: ApiRemedialGetRehabTriggersGet$Json$Params, context?: HttpContext): Observable<Array<RehabTrigger>> {
    return this.apiRemedialGetRehabTriggersGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<RehabTrigger>>): Array<RehabTrigger> => r.body)
    );
  }

}
