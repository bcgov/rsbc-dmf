/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ChefsSubmission } from '../../models/chefs-submission';

export interface ApiChefsSubmissionPut$Params {
  body?: ChefsSubmission;
}

export function apiChefsSubmissionPut(
  http: HttpClient,
  rootUrl: string,
  params?: ApiChefsSubmissionPut$Params,
  context?: HttpContext,
): Observable<StrictHttpResponse<ChefsSubmission>> {
  console.log('*******************apiChefsSubmissionPut*******************');
  const rb = new RequestBuilder(rootUrl, apiChefsSubmissionPut.PATH, 'put');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }
  console.log(rb);
  console.log('*******************apiChefsSubmissionPut*******************');
  return http
    .request(
      rb.build({ responseType: 'json', accept: 'application/json', context }),
    )
    .pipe(
      filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        console.log(
          '*******************apiChefsSubmissionPut RESPONSE*******************',
        );
        console.log(r);
        console.log(
          '*******************apiChefsSubmissionPut RESPONSE*******************',
        );
        return r as StrictHttpResponse<any>;
      }),
    );
}

apiChefsSubmissionPut.PATH = '/api/Chefs/submission';
