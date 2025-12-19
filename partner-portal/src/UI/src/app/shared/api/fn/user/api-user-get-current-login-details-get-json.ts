/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';


export interface ApiUserGetCurrentLoginDetailsGet$Json$Params {
}

export function apiUserGetCurrentLoginDetailsGet$Json(http: HttpClient, rootUrl: string, params?: ApiUserGetCurrentLoginDetailsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<string>>> {
  const rb = new RequestBuilder(rootUrl, apiUserGetCurrentLoginDetailsGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<string>>;
    })
  );
}

apiUserGetCurrentLoginDetailsGet$Json.PATH = '/api/User/getCurrentLoginDetails';
