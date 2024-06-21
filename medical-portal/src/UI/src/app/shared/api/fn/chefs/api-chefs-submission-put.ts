/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ChefsSubmission } from '../../models/chefs-submission';

export interface ApiChefsSubmissionPut$Params {
  caseId: string;
  body: ChefsSubmission;
}

export function apiChefsSubmissionPut(
  http: HttpClient,
  rootUrl: string,
  params?: ApiChefsSubmissionPut$Params,
  context?: HttpContext,
): Observable<StrictHttpResponse<ChefsSubmission>> {
  const rb = new RequestBuilder(rootUrl, apiChefsSubmissionPut.PATH, 'put');
  if (params) {
    rb.query('caseId', params.caseId, {});
    rb.body(params.body, 'application/*+json');
  }

  return http
    .request(rb.build({ responseType: 'text', accept: '*/*', context }))
    .pipe(
      filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<any>;
      }),
    );
}

apiChefsSubmissionPut.PATH = '/api/Chefs/submission';
