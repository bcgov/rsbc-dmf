/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DriverUpdate2 } from '../../models/driver-update-2';
import { OkResult2 } from '../../models/ok-result-2';

export interface ApiProfileDriverPut$Json$Params {
      body?: DriverUpdate2
}

export function apiProfileDriverPut$Json(http: HttpClient, rootUrl: string, params?: ApiProfileDriverPut$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult2>> {
  const rb = new RequestBuilder(rootUrl, apiProfileDriverPut$Json.PATH, 'put');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<OkResult2>;
    })
  );
}

apiProfileDriverPut$Json.PATH = '/api/Profile/driver';
