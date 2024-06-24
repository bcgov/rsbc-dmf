/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseStatus } from '../../models/case-status';

export interface IcbcCasesGet$Json$Params {
  caseId?: string;
}

export function icbcCasesGet$Json(http: HttpClient, rootUrl: string, params?: IcbcCasesGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseStatus>> {
  const rb = new RequestBuilder(rootUrl, icbcCasesGet$Json.PATH, 'get');
  if (params) {
    rb.query('caseId', params.caseId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CaseStatus>;
    })
  );
}

icbcCasesGet$Json.PATH = '/Icbc/Cases';
