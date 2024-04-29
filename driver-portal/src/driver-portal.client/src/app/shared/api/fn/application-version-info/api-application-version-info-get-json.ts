/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ApplicationVersionInfo } from '../../models/application-version-info';

export interface ApiApplicationVersionInfoGet$Json$Params {
}

export function apiApplicationVersionInfoGet$Json(http: HttpClient, rootUrl: string, params?: ApiApplicationVersionInfoGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<ApplicationVersionInfo>> {
  const rb = new RequestBuilder(rootUrl, apiApplicationVersionInfoGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ApplicationVersionInfo>;
    })
  );
}

apiApplicationVersionInfoGet$Json.PATH = '/api/ApplicationVersionInfo';
