/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDetail } from '../../models/case-detail';

export interface ApiCasesDriverIdClosedGet$Json$Params {
  driverId: string;
}

export function apiCasesDriverIdClosedGet$Json(http: HttpClient, rootUrl: string, params: ApiCasesDriverIdClosedGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDetail>>> {
  const rb = new RequestBuilder(rootUrl, apiCasesDriverIdClosedGet$Json.PATH, 'get');
  if (params) {
    rb.path('driverId', params.driverId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<CaseDetail>>;
    })
  );
}

apiCasesDriverIdClosedGet$Json.PATH = '/api/Cases/{driverId}/Closed';
