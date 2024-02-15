/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OkResult } from '../../models/ok-result';

export interface ApiDocumentUploadPost$Json$Params {
  body?: {
    file: Blob;
  };
}

export function apiDocumentUploadPost$Json(
  http: HttpClient,
  rootUrl: string,
  params?: ApiDocumentUploadPost$Json$Params,
  context?: HttpContext
): Observable<StrictHttpResponse<OkResult>> {
  const rb = new RequestBuilder(
    rootUrl,
    apiDocumentUploadPost$Json.PATH,
    'post'
  );
  if (params) {
    rb.body(params.body, 'multipart/form-data');
  }

  return http
    .request(
      rb.build({ responseType: 'json', accept: 'application/json', context })
    )
    .pipe(
      filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<OkResult>;
      })
    );
}

apiDocumentUploadPost$Json.PATH = '/api/Document/upload';
