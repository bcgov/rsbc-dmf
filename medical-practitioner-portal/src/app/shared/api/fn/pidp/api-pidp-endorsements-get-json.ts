/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Endorsement } from '../../models/endorsement';

export interface ApiPidpEndorsementsGet$Json$Params {
}

export function apiPidpEndorsementsGet$Json(http: HttpClient, rootUrl: string, params?: ApiPidpEndorsementsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Endorsement>>> {
  const rb = new RequestBuilder(rootUrl, apiPidpEndorsementsGet$Json.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<Endorsement>>;
    })
  );
}

apiPidpEndorsementsGet$Json.PATH = '/api/Pidp/endorsements';
