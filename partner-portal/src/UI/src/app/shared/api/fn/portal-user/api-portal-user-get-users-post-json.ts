/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { User } from '../../models/user';
import { UsersSearchRequest } from '../../models/users-search-request';

export interface ApiPortalUserGetUsersPost$Json$Params {
      body?: UsersSearchRequest
}

export function apiPortalUserGetUsersPost$Json(http: HttpClient, rootUrl: string, params?: ApiPortalUserGetUsersPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<User>>> {
  const rb = new RequestBuilder(rootUrl, apiPortalUserGetUsersPost$Json.PATH, 'post');
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

apiPortalUserGetUsersPost$Json.PATH = '/api/PortalUser/getUsers';
