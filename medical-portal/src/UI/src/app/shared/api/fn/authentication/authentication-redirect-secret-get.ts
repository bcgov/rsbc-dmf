/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';


export interface AuthenticationRedirectSecretGet$Params {
  secret: string;
  redirect_uri?: string;
  response_type?: string;
  state?: string;
}

export function authenticationRedirectSecretGet(http: HttpClient, rootUrl: string, params: AuthenticationRedirectSecretGet$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
  const rb = new RequestBuilder(rootUrl, authenticationRedirectSecretGet.PATH, 'get');
  if (params) {
    rb.path('secret', params.secret, {});
    rb.query('redirect_uri', params.redirect_uri, {});
    rb.query('response_type', params.response_type, {});
    rb.query('state', params.state, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: '*/*', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
    })
  );
}

authenticationRedirectSecretGet.PATH = '/Authentication/redirect/{secret}';
