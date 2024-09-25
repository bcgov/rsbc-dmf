/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DocumentSubType } from '../../models/document-sub-type';

export interface ApiDocumentTypeDocumentSubTypeGet$Json$Params {
}

export function apiDocumentTypeDocumentSubTypeGet$Json(http: HttpClient, rootUrl: string, params?: ApiDocumentTypeDocumentSubTypeGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DocumentSubType>>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentTypeDocumentSubTypeGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<DocumentSubType>>;
    })
  );
}

apiDocumentTypeDocumentSubTypeGet$Json.PATH = '/api/DocumentType/documentSubType';
