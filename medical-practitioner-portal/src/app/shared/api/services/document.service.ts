/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDocumentMyDmersGet$Json } from '../fn/document/api-document-my-dmers-get-json';
import { ApiDocumentMyDmersGet$Json$Params } from '../fn/document/api-document-my-dmers-get-json';
import { apiDocumentMyDmersGet$Plain } from '../fn/document/api-document-my-dmers-get-plain';
import { ApiDocumentMyDmersGet$Plain$Params } from '../fn/document/api-document-my-dmers-get-plain';
import { CaseDocument } from '../models/case-document';

@Injectable({ providedIn: 'root' })
export class DocumentService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiDocumentMyDmersGet()` */
  static readonly ApiDocumentMyDmersGetPath = '/api/Document/MyDmers';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentMyDmersGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentMyDmersGet$Plain$Response(params?: ApiDocumentMyDmersGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDocument>>> {
    return apiDocumentMyDmersGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentMyDmersGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentMyDmersGet$Plain(params?: ApiDocumentMyDmersGet$Plain$Params, context?: HttpContext): Observable<Array<CaseDocument>> {
    return this.apiDocumentMyDmersGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<CaseDocument>>): Array<CaseDocument> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentMyDmersGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentMyDmersGet$Json$Response(params?: ApiDocumentMyDmersGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDocument>>> {
    return apiDocumentMyDmersGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentMyDmersGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentMyDmersGet$Json(params?: ApiDocumentMyDmersGet$Json$Params, context?: HttpContext): Observable<Array<CaseDocument>> {
    return this.apiDocumentMyDmersGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<CaseDocument>>): Array<CaseDocument> => r.body)
    );
  }

}
