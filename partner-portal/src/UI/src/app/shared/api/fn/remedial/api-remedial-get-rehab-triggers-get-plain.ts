/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { RehabTrigger } from '../../models/rehab-trigger';

export interface ApiRemedialGetRehabTriggersGet$Plain$Params {
}

export function apiRemedialGetRehabTriggersGet$Plain(http: HttpClient, rootUrl: string, params?: ApiRemedialGetRehabTriggersGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<RehabTrigger>>> {
  const rb = new RequestBuilder(rootUrl, apiRemedialGetRehabTriggersGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<RehabTrigger>>;
    })
  );
}

apiRemedialGetRehabTriggersGet$Plain.PATH = '/api/Remedial/getRehabTriggers';
