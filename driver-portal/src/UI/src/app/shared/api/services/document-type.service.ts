/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDocumentTypeDriverGet$Json } from '../fn/document-type/api-document-type-driver-get-json';
import { ApiDocumentTypeDriverGet$Json$Params } from '../fn/document-type/api-document-type-driver-get-json';
import { apiDocumentTypeDriverGet$Plain } from '../fn/document-type/api-document-type-driver-get-plain';
import { ApiDocumentTypeDriverGet$Plain$Params } from '../fn/document-type/api-document-type-driver-get-plain';
import { DocumentSubType } from '../models/document-sub-type';

@Injectable({ providedIn: 'root' })
export class DocumentTypeService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiDocumentTypeDriverGet()` */
  static readonly ApiDocumentTypeDriverGetPath = '/api/DocumentType/driver';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentTypeDriverGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentTypeDriverGet$Plain$Response(params?: ApiDocumentTypeDriverGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DocumentSubType>>> {
    return apiDocumentTypeDriverGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentTypeDriverGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentTypeDriverGet$Plain(params?: ApiDocumentTypeDriverGet$Plain$Params, context?: HttpContext): Observable<Array<DocumentSubType>> {
    return this.apiDocumentTypeDriverGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<DocumentSubType>>): Array<DocumentSubType> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentTypeDriverGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentTypeDriverGet$Json$Response(params?: ApiDocumentTypeDriverGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DocumentSubType>>> {
    return apiDocumentTypeDriverGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentTypeDriverGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentTypeDriverGet$Json(params?: ApiDocumentTypeDriverGet$Json$Params, context?: HttpContext): Observable<Array<DocumentSubType>> {
    return this.apiDocumentTypeDriverGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<DocumentSubType>>): Array<DocumentSubType> => r.body)
    );
  }

}
