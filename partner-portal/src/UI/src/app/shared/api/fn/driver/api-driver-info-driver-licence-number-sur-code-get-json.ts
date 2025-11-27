/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Driver } from '../../models/driver';

export interface ApiDriverInfoDriverLicenceNumberSurCodeGet$Json$Params {
  driverLicenceNumber: string;
  surCode: string;
}

export function apiDriverInfoDriverLicenceNumberSurCodeGet$Json(http: HttpClient, rootUrl: string, params: ApiDriverInfoDriverLicenceNumberSurCodeGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Driver>> {
  const rb = new RequestBuilder(rootUrl, apiDriverInfoDriverLicenceNumberSurCodeGet$Json.PATH, 'get');
  if (params) {
    rb.path('driverLicenceNumber', params.driverLicenceNumber, {});
    rb.path('surCode', params.surCode, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'text/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Driver>;
    })
  );
}

apiDriverInfoDriverLicenceNumberSurCodeGet$Json.PATH = '/api/Driver/info/{driverLicenceNumber}/{surCode}';
