/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ChefsSubmission } from '../../models/chefs-submission';

export interface ApiChefsSubmissionGet$Plain$Params {
  caseId?: string;
  status?: string;
}

export function apiChefsSubmissionGet$Plain(http: HttpClient, rootUrl: string, params?: ApiChefsSubmissionGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsSubmission>> {
  const rb = new RequestBuilder(rootUrl, apiChefsSubmissionGet$Plain.PATH, 'get');
  if (params) {
    rb.query('caseId', params.caseId, {});
    rb.query('status', params.status, {});
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

apiChefsSubmissionGet$Plain.PATH = '/api/Chefs/submission';
