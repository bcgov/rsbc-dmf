/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseSearch } from '../../models/case-search';

export interface ApiCasesSearchIdCodeSurCodeGet$Json$Params {
  idCode: string;
  surCode: string;
}

export function apiCasesSearchIdCodeSurCodeGet$Json(http: HttpClient, rootUrl: string, params: ApiCasesSearchIdCodeSurCodeGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseSearch>> {
  const rb = new RequestBuilder(rootUrl, apiCasesSearchIdCodeSurCodeGet$Json.PATH, 'get');
  if (params) {
    rb.path('idCode', params.idCode, {});
    rb.path('surCode', params.surCode, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CaseSearch>;
    })
  );
}

apiCasesSearchIdCodeSurCodeGet$Json.PATH = '/api/Cases/search/{idCode}/{surCode}';
