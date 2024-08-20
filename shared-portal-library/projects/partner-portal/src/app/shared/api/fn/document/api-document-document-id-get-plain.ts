/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';


export interface ApiDocumentDocumentIdGet$Plain$Params {
  documentId: string;
}

export function apiDocumentDocumentIdGet$Plain(http: HttpClient, rootUrl: string, params: ApiDocumentDocumentIdGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Blob>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentDocumentIdGet$Plain.PATH, 'get');
  if (params) {
    rb.path('documentId', params.documentId, {});
  }

  return http.request(
    rb.build({ responseType: 'blob', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Blob>;
    })
  );
}

apiDocumentDocumentIdGet$Plain.PATH = '/api/Document/{documentId}';
