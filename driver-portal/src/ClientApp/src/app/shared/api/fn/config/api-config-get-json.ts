/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Configuration } from '../../models/configuration';

export interface ApiConfigGet$Json$Params {
}

export function apiConfigGet$Json(http: HttpClient, rootUrl: string, params?: ApiConfigGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Configuration>> {
  const rb = new RequestBuilder(rootUrl, apiConfigGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Configuration>;
    })
  );
}

apiConfigGet$Json.PATH = '/driver-portal/api/Config';
