/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiPidpEndorsementsGet } from '../fn/pidp/api-pidp-endorsements-get';
import { ApiPidpEndorsementsGet$Params } from '../fn/pidp/api-pidp-endorsements-get';

@Injectable({ providedIn: 'root' })
export class PidpService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiPidpEndorsementsGet()` */
  static readonly ApiPidpEndorsementsGetPath = '/api/Pidp/endorsements';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPidpEndorsementsGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPidpEndorsementsGet$Response(params?: ApiPidpEndorsementsGet$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiPidpEndorsementsGet(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPidpEndorsementsGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPidpEndorsementsGet(params?: ApiPidpEndorsementsGet$Params, context?: HttpContext): Observable<void> {
    return this.apiPidpEndorsementsGet$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

}
