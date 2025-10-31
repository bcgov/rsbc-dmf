/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { IgnitionInterlock } from '../../models/ignition-interlock';

export interface ApiRemedialGetIgnitionInterlockGet$Plain$Params {
}

export function apiRemedialGetIgnitionInterlockGet$Plain(http: HttpClient, rootUrl: string, params?: ApiRemedialGetIgnitionInterlockGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<IgnitionInterlock>>> {
  const rb = new RequestBuilder(rootUrl, apiRemedialGetIgnitionInterlockGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<IgnitionInterlock>>;
    })
  );
}

apiRemedialGetIgnitionInterlockGet$Plain.PATH = '/api/Remedial/getIgnitionInterlock';
