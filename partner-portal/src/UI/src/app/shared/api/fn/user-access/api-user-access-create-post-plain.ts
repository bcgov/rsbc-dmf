/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OkResult } from '../../models/ok-result';
import { UserAccessRequest } from '../../models/user-access-request';

export interface ApiUserAccessCreatePost$Plain$Params {
      body?: UserAccessRequest
}

export function apiUserAccessCreatePost$Plain(http: HttpClient, rootUrl: string, params?: ApiUserAccessCreatePost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
  const rb = new RequestBuilder(rootUrl, apiUserAccessCreatePost$Plain.PATH, 'post');
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

apiUserAccessCreatePost$Plain.PATH = '/api/UserAccess/create';
