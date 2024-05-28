/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiChefsSubmissionPut } from '../fn/chefs/api-chefs-submission-json-post';
import { ApiChefsSubmissionPut$Params } from '../fn/chefs/api-chefs-submission-json-post';
import { ChefsSubmission } from '../models/chefs-submission';

// import { ChefsSubmission } from '../models/chefs-submission';

@Injectable({ providedIn: 'root' })
export class ChefsService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiChefsSubmissionPut()` */
  static readonly ApiChefsSubmissionPutPath = '/api/Chefs/submission';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiChefsSubmissionPut()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  apiChefsSubmissionPut$Response(
    params?: ApiChefsSubmissionPut$Params,
    context?: HttpContext
  ): Observable<StrictHttpResponse<ChefsSubmission>> {
    return apiChefsSubmissionPut(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiChefsSubmissionPut$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  apiChefsSubmissionPut(
    params?: ApiChefsSubmissionPut$Params,
    context?: HttpContext
  ): Observable<ChefsSubmission> {
    return this.apiChefsSubmissionPut$Response(params, context).pipe(
      map((r: StrictHttpResponse<ChefsSubmission>): ChefsSubmission => r.body)
    );
  }
}
