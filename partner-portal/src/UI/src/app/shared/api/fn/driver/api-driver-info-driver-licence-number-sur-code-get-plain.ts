/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Driver } from '../../models/driver';

export interface ApiDriverInfoDriverLicenceNumberSurCodeGet$Plain$Params {
  driverLicenceNumber: string;
  surCode: string;
}

export function apiDriverInfoDriverLicenceNumberSurCodeGet$Plain(http: HttpClient, rootUrl: string, params: ApiDriverInfoDriverLicenceNumberSurCodeGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Driver>> {
  const rb = new RequestBuilder(rootUrl, apiDriverInfoDriverLicenceNumberSurCodeGet$Plain.PATH, 'get');
  if (params) {
    rb.path('driverLicenceNumber', params.driverLicenceNumber, {});
    rb.path('surCode', params.surCode, {});
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Driver>;
    })
  );
}

apiDriverInfoDriverLicenceNumberSurCodeGet$Plain.PATH = '/api/Driver/info/{driverLicenceNumber}/{surCode}';
