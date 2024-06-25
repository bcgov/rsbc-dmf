/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ChefsSubmission } from '../../models/chefs-submission';

export interface ApiChefsSubmissionGet$Json$Params {
  caseId?: string;
  status?: string;
}

export function apiChefsSubmissionGet$Json(http: HttpClient, rootUrl: string, params?: ApiChefsSubmissionGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<ChefsSubmission>> {
  const rb = new RequestBuilder(rootUrl, apiChefsSubmissionGet$Json.PATH, 'get');
  if (params) {
    rb.query('caseId', params.caseId, {});
    rb.query('status', params.status, {});
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

apiChefsSubmissionGet$Json.PATH = '/api/Chefs/submission';
