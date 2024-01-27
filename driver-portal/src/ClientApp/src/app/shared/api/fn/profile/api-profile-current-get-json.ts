/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { UserProfile } from '../../models/user-profile';

export interface ApiProfileCurrentGet$Json$Params {
}

export function apiProfileCurrentGet$Json(http: HttpClient, rootUrl: string, params?: ApiProfileCurrentGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<UserProfile>> {
  const rb = new RequestBuilder(rootUrl, apiProfileCurrentGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<UserProfile>;
    })
  );
}

apiProfileCurrentGet$Json.PATH = '/driver-portal/api/Profile/current';
