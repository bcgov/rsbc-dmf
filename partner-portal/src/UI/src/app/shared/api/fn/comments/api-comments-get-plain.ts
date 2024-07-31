/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseCallback } from '../../models/case-callback';

export interface ApiCommentsGet$Plain$Params {
}

export function apiCommentsGet$Plain(http: HttpClient, rootUrl: string, params?: ApiCommentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseCallback>>> {
  const rb = new RequestBuilder(rootUrl, apiCommentsGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<CaseCallback>>;
    })
  );
}

apiCommentsGet$Plain.PATH = '/api/Comments';
