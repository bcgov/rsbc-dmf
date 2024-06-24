/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { SubmissionStatus } from '../../models/submission-status';

export interface ApiChefsSubmissionGet$Params {
  caseId?: string;
  status?: SubmissionStatus;
}

export function apiChefsSubmissionGet(http: HttpClient, rootUrl: string, params?: ApiChefsSubmissionGet$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
  const rb = new RequestBuilder(rootUrl, apiChefsSubmissionGet.PATH, 'get');
  if (params) {
    rb.query('caseId', params.caseId, {});
    rb.query('status', params.status, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: '*/*', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
    })
  );
}

apiChefsSubmissionGet.PATH = '/api/Chefs/submission';
