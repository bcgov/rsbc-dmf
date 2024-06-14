/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { UserProfile } from '../../models/user-profile';

export interface ApiProfileCurrentGet$Plain$Params {
}

export function apiProfileCurrentGet$Plain(http: HttpClient, rootUrl: string, params?: ApiProfileCurrentGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<UserProfile>> {
  const rb = new RequestBuilder(rootUrl, apiProfileCurrentGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<UserProfile>;
    })
  );
}

apiProfileCurrentGet$Plain.PATH = '/api/Profile/current';
