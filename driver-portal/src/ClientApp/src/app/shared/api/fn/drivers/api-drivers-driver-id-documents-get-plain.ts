/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDocuments } from '../../models/case-documents';

export interface ApiDriversDriverIdDocumentsGet$Plain$Params {
  driverId: string;
}

export function apiDriversDriverIdDocumentsGet$Plain(http: HttpClient, rootUrl: string, params: ApiDriversDriverIdDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
  const rb = new RequestBuilder(rootUrl, apiDriversDriverIdDocumentsGet$Plain.PATH, 'get');
  if (params) {
    rb.path('driverId', params.driverId, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CaseDocuments>;
    })
  );
}

apiDriversDriverIdDocumentsGet$Plain.PATH = '/api/Drivers/{driverId}/Documents';
