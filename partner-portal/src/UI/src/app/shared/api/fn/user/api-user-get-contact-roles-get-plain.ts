/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { UserRole } from '../../models/user-role';

export interface ApiUserGetContactRolesGet$Plain$Params {
}

export function apiUserGetContactRolesGet$Plain(http: HttpClient, rootUrl: string, params?: ApiUserGetContactRolesGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<UserRole>>> {
  const rb = new RequestBuilder(rootUrl, apiUserGetContactRolesGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<UserRole>>;
    })
  );
}

apiUserGetContactRolesGet$Plain.PATH = '/api/User/getContactRoles';
