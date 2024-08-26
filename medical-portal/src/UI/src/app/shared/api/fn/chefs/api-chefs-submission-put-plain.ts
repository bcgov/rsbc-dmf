/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ChefsSubmission } from '../../models/chefs-submission';

export interface ApiChefsSubmissionPut$Plain$Params {
  caseId?: string;
  documentId?: string;
      body?: ChefsSubmission
}

export function apiChefsSubmissionPut$Plain(http: HttpClient, rootUrl: string, params?: ApiChefsSubmissionPut$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsSubmission>> {
  const rb = new RequestBuilder(rootUrl, apiChefsSubmissionPut$Plain.PATH, 'put');
  if (params) {
    rb.query('caseId', params.caseId, {});
    rb.query('documentId', params.documentId, {});
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ChefsSubmission>;
    })
  );
}

apiChefsSubmissionPut$Plain.PATH = '/api/Chefs/submission';
