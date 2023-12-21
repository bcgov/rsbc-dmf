/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ApplicationVersionInfo } from '../../models/application-version-info';

export interface ApiApplicationVersionInfoGet$Plain$Params {
}

export function apiApplicationVersionInfoGet$Plain(http: HttpClient, rootUrl: string, params?: ApiApplicationVersionInfoGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<ApplicationVersionInfo>> {
  const rb = new RequestBuilder(rootUrl, apiApplicationVersionInfoGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ApplicationVersionInfo>;
    })
  );
}

apiApplicationVersionInfoGet$Plain.PATH = '/api/ApplicationVersionInfo';
