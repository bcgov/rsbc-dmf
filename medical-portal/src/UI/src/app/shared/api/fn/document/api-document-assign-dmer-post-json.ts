/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DmerDocument } from '../../models/dmer-document';

export interface ApiDocumentAssignDmerPost$Json$Params {
  documentId?: string;
  loginId?: string;
}

export function apiDocumentAssignDmerPost$Json(http: HttpClient, rootUrl: string, params?: ApiDocumentAssignDmerPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<DmerDocument>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentAssignDmerPost$Json.PATH, 'post');
  if (params) {
    rb.query('documentId', params.documentId, {});
    rb.query('loginId', params.loginId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<DmerDocument>;
    })
  );
}

apiDocumentAssignDmerPost$Json.PATH = '/api/Document/assignDmer';
