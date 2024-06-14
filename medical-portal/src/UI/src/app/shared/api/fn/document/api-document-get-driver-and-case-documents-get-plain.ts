/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Document } from '../../models/document';

export interface ApiDocumentGetDriverAndCaseDocumentsGet$Plain$Params {
  caseId: string;
}

export function apiDocumentGetDriverAndCaseDocumentsGet$Plain(http: HttpClient, rootUrl: string, params: ApiDocumentGetDriverAndCaseDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentGetDriverAndCaseDocumentsGet$Plain.PATH, 'get');
  if (params) {
    rb.path('caseId', params.caseId, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<Document>>;
    })
  );
}

apiDocumentGetDriverAndCaseDocumentsGet$Plain.PATH = '/api/Document/GetDriverAndCaseDocuments';
