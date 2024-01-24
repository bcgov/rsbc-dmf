/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Document } from '../../models/document';

export interface ApiDriverAllDocumentsGet$Plain$Params {
}

export function apiDriverAllDocumentsGet$Plain(http: HttpClient, rootUrl: string, params?: ApiDriverAllDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
  const rb = new RequestBuilder(rootUrl, apiDriverAllDocumentsGet$Plain.PATH, 'get');
  if (params) {
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

apiDriverAllDocumentsGet$Plain.PATH = '/api/Driver/AllDocuments';
