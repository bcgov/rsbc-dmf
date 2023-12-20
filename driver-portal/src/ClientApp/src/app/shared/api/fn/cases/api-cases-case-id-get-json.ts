/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDetail } from '../../models/case-detail';

export interface ApiCasesCaseIdGet$Json$Params {
  caseId: string;
}

export function apiCasesCaseIdGet$Json(http: HttpClient, rootUrl: string, params: ApiCasesCaseIdGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDetail>> {
  const rb = new RequestBuilder(rootUrl, apiCasesCaseIdGet$Json.PATH, 'get');
  if (params) {
    rb.path('caseId', params.caseId, {});
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

apiCasesCaseIdGet$Json.PATH = '/api/Cases/{caseId}';
