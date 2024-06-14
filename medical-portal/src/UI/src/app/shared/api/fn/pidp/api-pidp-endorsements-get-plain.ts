/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Endorsement } from '../../models/endorsement';

export interface ApiPidpEndorsementsGet$Plain$Params {
}

export function apiPidpEndorsementsGet$Plain(http: HttpClient, rootUrl: string, params?: ApiPidpEndorsementsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Endorsement>>> {
  const rb = new RequestBuilder(rootUrl, apiPidpEndorsementsGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<Endorsement>>;
    })
  );
}

apiPidpEndorsementsGet$Plain.PATH = '/api/Pidp/endorsements';
