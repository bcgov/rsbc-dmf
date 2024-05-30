/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDocument } from '../../models/case-document';

export interface ApiDocumentDriverIdAllDocumentsGet$Json$Params {
  driverId: string;
}

export function apiDocumentDriverIdAllDocumentsGet$Json(http: HttpClient, rootUrl: string, params: ApiDocumentDriverIdAllDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDocument>>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentDriverIdAllDocumentsGet$Json.PATH, 'get');
  if (params) {
    rb.path('driverId', params.driverId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<CaseDocument>>;
    })
  );
}

apiDocumentDriverIdAllDocumentsGet$Json.PATH = '/api/Document/{driverId}/AllDocuments';
