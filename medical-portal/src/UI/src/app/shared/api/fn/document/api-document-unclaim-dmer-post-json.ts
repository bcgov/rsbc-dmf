/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { CaseDocument } from '../../models/case-document';

export interface ApiDocumentUnclaimDmerPost$Json$Params {
  documentId: string;
}

export function apiDocumentUnclaimDmerPost$Json(http: HttpClient, rootUrl: string, params: ApiDocumentUnclaimDmerPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocument>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentUnclaimDmerPost$Json.PATH, 'post');
  if (params) {
    rb.path('documentId', params.documentId, {});
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

apiDocumentUnclaimDmerPost$Json.PATH = '/api/Document/unclaimDmer';
