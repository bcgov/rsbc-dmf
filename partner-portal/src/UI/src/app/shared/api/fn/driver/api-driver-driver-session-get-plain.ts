/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { UserContext } from '../../models/user-context';

export interface ApiDriverDriverSessionGet$Plain$Params {
}

export function apiDriverDriverSessionGet$Plain(http: HttpClient, rootUrl: string, params?: ApiDriverDriverSessionGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<UserContext>> {
  const rb = new RequestBuilder(rootUrl, apiDriverDriverSessionGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<UserContext>;
    })
  );
}

apiDriverDriverSessionGet$Plain.PATH = '/api/Driver/driverSession';
