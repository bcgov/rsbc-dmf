/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDocumentDocumentIdGet$Json } from '../fn/document/api-document-document-id-get-json';
import { ApiDocumentDocumentIdGet$Json$Params } from '../fn/document/api-document-document-id-get-json';
import { apiDocumentDocumentIdGet$Plain } from '../fn/document/api-document-document-id-get-plain';
import { ApiDocumentDocumentIdGet$Plain$Params } from '../fn/document/api-document-document-id-get-plain';

@Injectable({ providedIn: 'root' })
export class DocumentService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiDocumentDocumentIdGet()` */
  static readonly ApiDocumentDocumentIdGetPath = '/api/Document/{documentId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentDocumentIdGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentDocumentIdGet$Plain$Response(params: ApiDocumentDocumentIdGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Blob>> {
    return apiDocumentDocumentIdGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentDocumentIdGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentDocumentIdGet$Plain(params: ApiDocumentDocumentIdGet$Plain$Params, context?: HttpContext): Observable<Blob> {
    return this.apiDocumentDocumentIdGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Blob>): Blob => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentDocumentIdGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentDocumentIdGet$Json$Response(params: ApiDocumentDocumentIdGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Blob>> {
    return apiDocumentDocumentIdGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentDocumentIdGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentDocumentIdGet$Json(params: ApiDocumentDocumentIdGet$Json$Params, context?: HttpContext): Observable<Blob> {
    return this.apiDocumentDocumentIdGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Blob>): Blob => r.body)
    );
  }

}
