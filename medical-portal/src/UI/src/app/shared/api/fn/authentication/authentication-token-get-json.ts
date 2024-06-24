/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';


export interface AuthenticationTokenGet$Json$Params {
  secret?: string;
}

export function authenticationTokenGet$Json(http: HttpClient, rootUrl: string, params?: AuthenticationTokenGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<string>> {
  const rb = new RequestBuilder(rootUrl, authenticationTokenGet$Json.PATH, 'get');
  if (params) {
    rb.query('secret', params.secret, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<string>;
    })
  );
}

authenticationTokenGet$Json.PATH = '/Authentication/token';
