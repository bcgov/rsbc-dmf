/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';
import { ChefsBundle } from '../../models/chefs-bundle';

export interface ApiChefsBundleGet$Params {
  caseId: string | null;
}

export function apiChefsBundleGet(
  http: HttpClient,
  rootUrl: string,
  params?: ApiChefsBundleGet$Params,
  context?: HttpContext,
): Observable<StrictHttpResponse<ChefsBundle>> {
  const rb = new RequestBuilder(rootUrl, apiChefsBundleGet.PATH, 'get');
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

apiChefsBundleGet.PATH = '/api/Chefs/bundle';
