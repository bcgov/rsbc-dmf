/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseSearch } from '../../models/case-search';

export interface ApiCasesSearchIdCodeGet$Plain$Params {
  idCode: string;
}

export function apiCasesSearchIdCodeGet$Plain(http: HttpClient, rootUrl: string, params: ApiCasesSearchIdCodeGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseSearch>> {
  const rb = new RequestBuilder(rootUrl, apiCasesSearchIdCodeGet$Plain.PATH, 'get');
  if (params) {
    rb.path('idCode', params.idCode, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CaseSearch>;
    })
  );
}

apiCasesSearchIdCodeGet$Plain.PATH = '/api/Cases/search/{idCode}';
