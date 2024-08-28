/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ChefsSubmission } from '../../models/chefs-submission';

export interface ApiChefsSubmissionPut$Json$Params {
  caseId?: string;
  documentId?: string;
      body?: ChefsSubmission
}

export function apiChefsSubmissionPut$Json(http: HttpClient, rootUrl: string, params?: ApiChefsSubmissionPut$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsSubmission>> {
  const rb = new RequestBuilder(rootUrl, apiChefsSubmissionPut$Json.PATH, 'put');
  if (params) {
    rb.query('caseId', params.caseId, {});
    rb.query('documentId', params.documentId, {});
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ChefsSubmission>;
    })
  );
}

apiChefsSubmissionPut$Json.PATH = '/api/Chefs/submission';
