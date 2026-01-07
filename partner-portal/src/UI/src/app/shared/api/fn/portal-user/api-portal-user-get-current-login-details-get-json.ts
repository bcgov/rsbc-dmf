/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CurrentLoginDetails } from '../../models/current-login-details';

export interface ApiPortalUserGetCurrentLoginDetailsGet$Json$Params {
}

export function apiPortalUserGetCurrentLoginDetailsGet$Json(http: HttpClient, rootUrl: string, params?: ApiPortalUserGetCurrentLoginDetailsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CurrentLoginDetails>> {
  const rb = new RequestBuilder(rootUrl, apiPortalUserGetCurrentLoginDetailsGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CurrentLoginDetails>;
    })
  );
}

apiPortalUserGetCurrentLoginDetailsGet$Json.PATH = '/api/PortalUser/getCurrentLoginDetails';
