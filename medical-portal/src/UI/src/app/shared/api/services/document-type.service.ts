/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDocumentTypeDocumentSubTypeGet$Json } from '../fn/document-type/api-document-type-document-sub-type-get-json';
import { ApiDocumentTypeDocumentSubTypeGet$Json$Params } from '../fn/document-type/api-document-type-document-sub-type-get-json';
import { apiDocumentTypeDocumentSubTypeGet$Plain } from '../fn/document-type/api-document-type-document-sub-type-get-plain';
import { ApiDocumentTypeDocumentSubTypeGet$Plain$Params } from '../fn/document-type/api-document-type-document-sub-type-get-plain';
import { DocumentSubTypes } from '../models/document-sub-types';

@Injectable({ providedIn: 'root' })
export class DocumentTypeService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiDocumentTypeDocumentSubTypeGet()` */
  static readonly ApiDocumentTypeDocumentSubTypeGetPath = '/api/DocumentType/documentSubType';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentTypeDocumentSubTypeGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentTypeDocumentSubTypeGet$Plain$Response(params?: ApiDocumentTypeDocumentSubTypeGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DocumentSubTypes>>> {
    return apiDocumentTypeDocumentSubTypeGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentTypeDocumentSubTypeGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentTypeDocumentSubTypeGet$Plain(params?: ApiDocumentTypeDocumentSubTypeGet$Plain$Params, context?: HttpContext): Observable<Array<DocumentSubTypes>> {
    return this.apiDocumentTypeDocumentSubTypeGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<DocumentSubTypes>>): Array<DocumentSubTypes> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentTypeDocumentSubTypeGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentTypeDocumentSubTypeGet$Json$Response(params?: ApiDocumentTypeDocumentSubTypeGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DocumentSubTypes>>> {
    return apiDocumentTypeDocumentSubTypeGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentTypeDocumentSubTypeGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentTypeDocumentSubTypeGet$Json(params?: ApiDocumentTypeDocumentSubTypeGet$Json$Params, context?: HttpContext): Observable<Array<DocumentSubTypes>> {
    return this.apiDocumentTypeDocumentSubTypeGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<DocumentSubTypes>>): Array<DocumentSubTypes> => r.body)
    );
  }

}
