/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { User } from '../../models/user';
import { UsersSearchRequest } from '../../models/users-search-request';

export interface ApiUserGetUsersPost$Json$Params {
      body?: UsersSearchRequest
}

export function apiUserGetUsersPost$Json(http: HttpClient, rootUrl: string, params?: ApiUserGetUsersPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<User>>> {
  const rb = new RequestBuilder(rootUrl, apiUserGetUsersPost$Json.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<User>>;
    })
  );
}

apiUserGetUsersPost$Json.PATH = '/api/User/getUsers';
