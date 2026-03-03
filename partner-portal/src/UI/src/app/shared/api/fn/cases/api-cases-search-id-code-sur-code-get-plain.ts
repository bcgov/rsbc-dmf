/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseSearch } from '../../models/case-search';

export interface ApiCasesSearchIdCodeSurCodeGet$Plain$Params {
  idCode: string;
  surCode: string;
}

export function apiCasesSearchIdCodeSurCodeGet$Plain(http: HttpClient, rootUrl: string, params: ApiCasesSearchIdCodeSurCodeGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseSearch>> {
  const rb = new RequestBuilder(rootUrl, apiCasesSearchIdCodeSurCodeGet$Plain.PATH, 'get');
  if (params) {
    rb.path('idCode', params.idCode, {});
    rb.path('surCode', params.surCode, {});
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

apiCasesSearchIdCodeSurCodeGet$Plain.PATH = '/api/Cases/search/{idCode}/{surCode}';
