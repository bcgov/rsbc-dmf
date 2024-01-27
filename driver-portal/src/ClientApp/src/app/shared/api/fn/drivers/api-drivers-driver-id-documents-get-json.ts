/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDocuments } from '../../models/case-documents';

export interface ApiDriversDriverIdDocumentsGet$Json$Params {
  driverId: string;
}

export function apiDriversDriverIdDocumentsGet$Json(http: HttpClient, rootUrl: string, params: ApiDriversDriverIdDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocuments>> {
  const rb = new RequestBuilder(rootUrl, apiDriversDriverIdDocumentsGet$Json.PATH, 'get');
  if (params) {
    rb.path('driverId', params.driverId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CaseDocuments>;
    })
  );
}

apiDriversDriverIdDocumentsGet$Json.PATH = '/driver-portal/api/Drivers/{driverId}/Documents';
