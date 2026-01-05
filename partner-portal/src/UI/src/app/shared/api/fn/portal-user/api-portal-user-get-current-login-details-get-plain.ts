/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';


export interface ApiPortalUserGetCurrentLoginDetailsGet$Plain$Params {
}

export function apiPortalUserGetCurrentLoginDetailsGet$Plain(http: HttpClient, rootUrl: string, params?: ApiPortalUserGetCurrentLoginDetailsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<string>>> {
  const rb = new RequestBuilder(rootUrl, apiPortalUserGetCurrentLoginDetailsGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<string>>;
    })
  );
}

apiPortalUserGetCurrentLoginDetailsGet$Plain.PATH = '/api/PortalUser/getCurrentLoginDetails';
