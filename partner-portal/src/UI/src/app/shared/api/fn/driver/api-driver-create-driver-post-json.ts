/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DriverCreateRecordRequest } from '../../models/driver-create-record-request';
import { DriverCreateRecordResponse } from '../../models/driver-create-record-response';

export interface ApiDriverCreateDriverPost$Json$Params {
      body?: DriverCreateRecordRequest
}

export function apiDriverCreateDriverPost$Json(http: HttpClient, rootUrl: string, params?: ApiDriverCreateDriverPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<DriverCreateRecordResponse>> {
  const rb = new RequestBuilder(rootUrl, apiDriverCreateDriverPost$Json.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<DriverCreateRecordResponse>;
    })
  );
}

apiDriverCreateDriverPost$Json.PATH = '/api/Driver/CreateDriver';
