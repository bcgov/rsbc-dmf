/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDocuments } from '../../models/case-documents';

export interface ApiDriverDriverIdDocumentsGet$Plain$Params {
  driverId: string;
}

export function apiDriverDriverIdDocumentsGet$Plain(http: HttpClient, rootUrl: string, params: ApiDriverDriverIdDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
  const rb = new RequestBuilder(rootUrl, apiDriverDriverIdDocumentsGet$Plain.PATH, 'get');
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

apiDriverDriverIdDocumentsGet$Plain.PATH = '/api/Driver/{driverId}/Documents';
