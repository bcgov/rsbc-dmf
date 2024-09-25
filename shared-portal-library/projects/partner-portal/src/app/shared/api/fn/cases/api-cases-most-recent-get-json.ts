/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDetail } from '../../models/case-detail';

export interface ApiCasesMostRecentGet$Json$Params {
}

export function apiCasesMostRecentGet$Json(http: HttpClient, rootUrl: string, params?: ApiCasesMostRecentGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDetail>> {
  const rb = new RequestBuilder(rootUrl, apiCasesMostRecentGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CaseDetail>;
    })
  );
}

apiCasesMostRecentGet$Json.PATH = '/api/Cases/MostRecent';
