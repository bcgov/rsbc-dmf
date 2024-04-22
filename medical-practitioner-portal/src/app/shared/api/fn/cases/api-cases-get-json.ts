/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DmerCaseListItem } from '../../models/dmer-case-list-item';

export interface ApiCasesGet$Json$Params {
  ByTitle?: string;
  ByDriverLicense?: string;
  ByClinicId?: string;
  ByStatus?: Array<string>;
}

export function apiCasesGet$Json(http: HttpClient, rootUrl: string, params?: ApiCasesGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DmerCaseListItem>>> {
  const rb = new RequestBuilder(rootUrl, apiCasesGet$Json.PATH, 'get');
  if (params) {
    rb.query('ByTitle', params.ByTitle, {});
    rb.query('ByDriverLicense', params.ByDriverLicense, {});
    rb.query('ByClinicId', params.ByClinicId, {});
    rb.query('ByStatus', params.ByStatus, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<DmerCaseListItem>>;
    })
  );
}

apiCasesGet$Json.PATH = '/api/Cases';
