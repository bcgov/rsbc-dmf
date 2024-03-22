/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DriverUpdate } from '../../models/driver-update';
import { OkResult } from '../../models/ok-result';

export interface ApiProfileDriverPut$Plain$Params {
      body?: DriverUpdate
}

export function apiProfileDriverPut$Plain(http: HttpClient, rootUrl: string, params?: ApiProfileDriverPut$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
  const rb = new RequestBuilder(rootUrl, apiProfileDriverPut$Plain.PATH, 'put');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<OkResult>;
    })
  );
}

apiProfileDriverPut$Plain.PATH = '/api/Profile/driver';
