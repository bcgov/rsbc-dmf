/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OkResult2 } from '../../models/ok-result-2';
import { UserRegistration2 } from '../../models/user-registration-2';

export interface ApiProfileRegisterPut$Plain$Params {
      body?: UserRegistration2
}

export function apiProfileRegisterPut$Plain(http: HttpClient, rootUrl: string, params?: ApiProfileRegisterPut$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult2>> {
  const rb = new RequestBuilder(rootUrl, apiProfileRegisterPut$Plain.PATH, 'put');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<OkResult2>;
    })
  );
}

apiProfileRegisterPut$Plain.PATH = '/api/Profile/register';
