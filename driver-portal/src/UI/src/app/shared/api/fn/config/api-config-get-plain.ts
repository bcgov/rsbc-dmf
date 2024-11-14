/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Configuration } from '../../models/configuration';

export interface ApiConfigGet$Plain$Params {
}

export function apiConfigGet$Plain(http: HttpClient, rootUrl: string, params?: ApiConfigGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Configuration>> {
  const rb = new RequestBuilder(rootUrl, apiConfigGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Configuration>;
    })
  );
}

apiConfigGet$Plain.PATH = '/api/Config';
