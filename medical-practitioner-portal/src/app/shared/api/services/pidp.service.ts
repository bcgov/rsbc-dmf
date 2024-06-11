/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiPidpEndorsementsGet$Json } from '../fn/pidp/api-pidp-endorsements-get-json';
import { ApiPidpEndorsementsGet$Json$Params } from '../fn/pidp/api-pidp-endorsements-get-json';
import { apiPidpEndorsementsGet$Plain } from '../fn/pidp/api-pidp-endorsements-get-plain';
import { ApiPidpEndorsementsGet$Plain$Params } from '../fn/pidp/api-pidp-endorsements-get-plain';
import { Endorsement } from '../models/endorsement';

@Injectable({ providedIn: 'root' })
export class PidpService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiPidpEndorsementsGet()` */
  static readonly ApiPidpEndorsementsGetPath = '/api/Pidp/endorsements';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPidpEndorsementsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPidpEndorsementsGet$Plain$Response(params?: ApiPidpEndorsementsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Endorsement>>> {
    return apiPidpEndorsementsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPidpEndorsementsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPidpEndorsementsGet$Plain(params?: ApiPidpEndorsementsGet$Plain$Params, context?: HttpContext): Observable<Array<Endorsement>> {
    return this.apiPidpEndorsementsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Endorsement>>): Array<Endorsement> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPidpEndorsementsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPidpEndorsementsGet$Json$Response(params?: ApiPidpEndorsementsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Endorsement>>> {
    return apiPidpEndorsementsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPidpEndorsementsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPidpEndorsementsGet$Json(params?: ApiPidpEndorsementsGet$Json$Params, context?: HttpContext): Observable<Array<Endorsement>> {
    return this.apiPidpEndorsementsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<Endorsement>>): Array<Endorsement> => r.body)
    );
  }

}
