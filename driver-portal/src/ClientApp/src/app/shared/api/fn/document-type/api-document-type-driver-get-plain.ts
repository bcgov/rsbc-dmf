/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DocumentSubType } from '../../models/document-sub-type';

export interface ApiDocumentTypeDriverGet$Plain$Params {
}

export function apiDocumentTypeDriverGet$Plain(http: HttpClient, rootUrl: string, params?: ApiDocumentTypeDriverGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DocumentSubType>>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentTypeDriverGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<DocumentSubType>>;
    })
  );
}

apiDocumentTypeDriverGet$Plain.PATH = '/api/DocumentType/driver';
