/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OkResult } from '../../models/ok-result';
import { UserRegistration } from '../../models/user-registration';

export interface ApiProfileRegisterPut$Plain$Params {
      body?: UserRegistration
}

export function apiProfileRegisterPut$Plain(http: HttpClient, rootUrl: string, params?: ApiProfileRegisterPut$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
  const rb = new RequestBuilder(rootUrl, apiProfileRegisterPut$Plain.PATH, 'put');
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

apiProfileRegisterPut$Plain.PATH = '/api/Profile/Register';
