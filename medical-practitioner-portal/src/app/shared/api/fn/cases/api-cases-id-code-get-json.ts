/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { PatientCase } from '../../models/patient-case';

export interface ApiCasesIdCodeGet$Json$Params {
  idCode: string;
}

export function apiCasesIdCodeGet$Json(http: HttpClient, rootUrl: string, params: ApiCasesIdCodeGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<PatientCase>> {
  const rb = new RequestBuilder(rootUrl, apiCasesIdCodeGet$Json.PATH, 'get');
  if (params) {
    rb.path('idCode', params.idCode, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<PatientCase>;
    })
  );
}

apiCasesIdCodeGet$Json.PATH = '/api/Cases/{idCode}';
