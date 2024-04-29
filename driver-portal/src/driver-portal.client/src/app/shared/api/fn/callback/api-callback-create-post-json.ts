/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Callback } from '../../models/callback';
import { OkResult } from '../../models/ok-result';

export interface ApiCallbackCreatePost$Json$Params {
      body?: Callback
}

export function apiCallbackCreatePost$Json(http: HttpClient, rootUrl: string, params?: ApiCallbackCreatePost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
  const rb = new RequestBuilder(rootUrl, apiCallbackCreatePost$Json.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<OkResult>;
    })
  );
}

apiCallbackCreatePost$Json.PATH = '/api/Callback/create';
