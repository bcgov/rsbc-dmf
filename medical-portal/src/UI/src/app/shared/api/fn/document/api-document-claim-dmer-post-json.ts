/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDocument } from '../../models/case-document';

export interface ApiDocumentClaimDmerPost$Json$Params {
  documentId?: string;
}

export function apiDocumentClaimDmerPost$Json(http: HttpClient, rootUrl: string, params?: ApiDocumentClaimDmerPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocument>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentClaimDmerPost$Json.PATH, 'post');
  if (params) {
    rb.query('documentId', params.documentId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<CaseDocument>;
    })
  );
}

apiDocumentClaimDmerPost$Json.PATH = '/api/Document/claimDmer';
