/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DmerDocument } from '../../models/dmer-document';

export interface ApiDocumentClaimDmerPost$Plain$Params {
  documentId?: string;
}

export function apiDocumentClaimDmerPost$Plain(http: HttpClient, rootUrl: string, params?: ApiDocumentClaimDmerPost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<DmerDocument>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentClaimDmerPost$Plain.PATH, 'post');
  if (params) {
    rb.query('documentId', params.documentId, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<DmerDocument>;
    })
  );
}

apiDocumentClaimDmerPost$Plain.PATH = '/api/Document/claimDmer';
