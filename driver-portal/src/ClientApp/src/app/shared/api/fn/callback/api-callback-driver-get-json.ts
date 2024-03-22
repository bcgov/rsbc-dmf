/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Callback2 } from '../../models/callback-2';

export interface ApiCallbackDriverGet$Json$Params {
}

export function apiCallbackDriverGet$Json(http: HttpClient, rootUrl: string, params?: ApiCallbackDriverGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Callback2>>> {
  const rb = new RequestBuilder(rootUrl, apiCallbackDriverGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<Callback2>>;
    })
  );
}

apiCallbackDriverGet$Json.PATH = '/api/Callback/driver';
