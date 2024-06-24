/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';


export interface AuthenticationTokenGet$Plain$Params {
  secret?: string;
}

export function authenticationTokenGet$Plain(http: HttpClient, rootUrl: string, params?: AuthenticationTokenGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<string>> {
  const rb = new RequestBuilder(rootUrl, authenticationTokenGet$Plain.PATH, 'get');
  if (params) {
    rb.query('secret', params.secret, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<string>;
    })
  );
}

authenticationTokenGet$Plain.PATH = '/Authentication/token';
