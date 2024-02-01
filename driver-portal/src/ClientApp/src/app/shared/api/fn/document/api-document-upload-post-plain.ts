/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OkResult } from '../../models/ok-result';

export interface ApiDocumentUploadPost$Plain$Params {
      body?: {
'ContentType'?: string;
'ContentDisposition'?: string;
'Headers'?: {
[key: string]: Array<string>;
};
'Length'?: number;
'Name'?: string;
'FileName'?: string;
}
}

export function apiDocumentUploadPost$Plain(http: HttpClient, rootUrl: string, params?: ApiDocumentUploadPost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<OkResult>> {
  const rb = new RequestBuilder(rootUrl, apiDocumentUploadPost$Plain.PATH, 'post');
  if (params) {
    rb.body(params.body, 'multipart/form-data');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<OkResult>;
    })
  );
}

apiDocumentUploadPost$Plain.PATH = '/api/Document/upload';
