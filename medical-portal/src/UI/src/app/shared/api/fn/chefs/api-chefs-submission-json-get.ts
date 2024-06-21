/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ChefsSubmission } from '../../models/chefs-submission';

export interface ApiChefsSubmissionGet$Params {
  caseId: string | null;
}

export function apiChefsSubmissionGet(
  http: HttpClient,
  rootUrl: string,
  params?: ApiChefsSubmissionGet$Params,
  context?: HttpContext,
): Observable<StrictHttpResponse<ChefsSubmission>> {
  const rb = new RequestBuilder(rootUrl, apiChefsSubmissionGet.PATH, 'get');
  if (params) {
    rb.query('caseId', params.caseId, {});
  }
  return http
    .request(
      rb.build({ responseType: 'json', accept: 'application/json', context }),
    )
    .pipe(
      filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<any>;
      }),
    );
}

apiChefsSubmissionGet.PATH = '/api/Chefs/submission';
