/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { UserRole } from '../../models/user-role';

export interface ApiPortalUserGetContactRolesGet$Json$Params {
}

export function apiPortalUserGetContactRolesGet$Json(http: HttpClient, rootUrl: string, params?: ApiPortalUserGetContactRolesGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<UserRole>>> {
  const rb = new RequestBuilder(rootUrl, apiPortalUserGetContactRolesGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<UserRole>>;
    })
  );
}

apiPortalUserGetContactRolesGet$Json.PATH = '/api/PortalUser/getContactRoles';
