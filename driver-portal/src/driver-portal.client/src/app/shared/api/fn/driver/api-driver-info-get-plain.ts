/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Document } from '../../models/document';

export interface ApiDriverInfoGet$Plain$Params {
}

export function apiDriverInfoGet$Plain(http: HttpClient, rootUrl: string, params?: ApiDriverInfoGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Document>>> {
  const rb = new RequestBuilder(rootUrl, apiDriverInfoGet$Plain.PATH, 'get');
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

apiDriverInfoGet$Plain.PATH = '/api/Driver/info';
