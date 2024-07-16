/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CallbackCancelRequest } from '../../models/callback-cancel-request';
import { OkResult } from '../../models/ok-result';

export interface ApiCallbackCancelPut$Plain$Params {
      body?: CallbackCancelRequest
}

export function apiCallbackCancelPut$Plain(http: HttpClient, rootUrl: string, params?: ApiCallbackCancelPut$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
  const rb = new RequestBuilder(rootUrl, apiCallbackCancelPut$Plain.PATH, 'put');
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

apiCallbackCancelPut$Plain.PATH = '/api/Callback/cancel';
