/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDetail } from '../../models/case-detail';

export interface ApiCasesMostRecentGet$Plain$Params {
}

export function apiCasesMostRecentGet$Plain(http: HttpClient, rootUrl: string, params?: ApiCasesMostRecentGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDetail>> {
  const rb = new RequestBuilder(rootUrl, apiCasesMostRecentGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CaseDetail>;
    })
  );
}

apiCasesMostRecentGet$Plain.PATH = '/api/Cases/MostRecent';
