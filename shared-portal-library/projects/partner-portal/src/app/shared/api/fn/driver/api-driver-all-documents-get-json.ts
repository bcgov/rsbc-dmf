/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Document } from '../../models/document';

export interface ApiDriverAllDocumentsGet$Json$Params {
}

export function apiDriverAllDocumentsGet$Json(http: HttpClient, rootUrl: string, params?: ApiDriverAllDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
  const rb = new RequestBuilder(rootUrl, apiDriverAllDocumentsGet$Json.PATH, 'get');
  if (params) {
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

apiDriverAllDocumentsGet$Json.PATH = '/api/Driver/AllDocuments';
