/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Document } from '../../models/document';

export interface ApiDocumentGetDriverAndCaseDocumentsGet$Json$Params {
  caseId: string;
}

export function apiDocumentGetDriverAndCaseDocumentsGet$Json(http: HttpClient, rootUrl: string, params: ApiDocumentGetDriverAndCaseDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentGetDriverAndCaseDocumentsGet$Json.PATH, 'get');
  if (params) {
    rb.path('caseId', params.caseId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<Document>>;
    })
  );
}

apiDocumentGetDriverAndCaseDocumentsGet$Json.PATH = '/api/Document/GetDriverAndCaseDocuments';
