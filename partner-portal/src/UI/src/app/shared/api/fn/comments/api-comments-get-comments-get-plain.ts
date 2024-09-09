/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Comment } from '../../models/comment';

export interface ApiCommentsGetCommentsGet$Plain$Params {
}

export function apiCommentsGetCommentsGet$Plain(http: HttpClient, rootUrl: string, params?: ApiCommentsGetCommentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<Comment>>> {
  const rb = new RequestBuilder(rootUrl, apiCommentsGetCommentsGet$Plain.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'text', accept: 'text/plain', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<Comment>>;
    })
  );
}

apiCommentsGetCommentsGet$Plain.PATH = '/api/Comments/getComments';
