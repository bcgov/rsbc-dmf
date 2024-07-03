/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiDocumentClaimDmerPost$Json } from '../fn/document/api-document-claim-dmer-post-json';
import { ApiDocumentClaimDmerPost$Json$Params } from '../fn/document/api-document-claim-dmer-post-json';
import { apiDocumentClaimDmerPost$Plain } from '../fn/document/api-document-claim-dmer-post-plain';
import { ApiDocumentClaimDmerPost$Plain$Params } from '../fn/document/api-document-claim-dmer-post-plain';
import { apiDocumentDriverIdAllDocumentsGet$Json } from '../fn/document/api-document-driver-id-all-documents-get-json';
import { ApiDocumentDriverIdAllDocumentsGet$Json$Params } from '../fn/document/api-document-driver-id-all-documents-get-json';
import { apiDocumentDriverIdAllDocumentsGet$Plain } from '../fn/document/api-document-driver-id-all-documents-get-plain';
import { ApiDocumentDriverIdAllDocumentsGet$Plain$Params } from '../fn/document/api-document-driver-id-all-documents-get-plain';
import { apiDocumentMyDmersGet$Json } from '../fn/document/api-document-my-dmers-get-json';
import { ApiDocumentMyDmersGet$Json$Params } from '../fn/document/api-document-my-dmers-get-json';
import { apiDocumentMyDmersGet$Plain } from '../fn/document/api-document-my-dmers-get-plain';
import { ApiDocumentMyDmersGet$Plain$Params } from '../fn/document/api-document-my-dmers-get-plain';
import { apiDocumentUnclaimDmerPost$Json } from '../fn/document/api-document-unclaim-dmer-post-json';
import { ApiDocumentUnclaimDmerPost$Json$Params } from '../fn/document/api-document-unclaim-dmer-post-json';
import { apiDocumentUnclaimDmerPost$Plain } from '../fn/document/api-document-unclaim-dmer-post-plain';
import { ApiDocumentUnclaimDmerPost$Plain$Params } from '../fn/document/api-document-unclaim-dmer-post-plain';
import { CaseDocument } from '../models/case-document';
import { DmerDocument } from '../models/dmer-document';

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
  apiDocumentMyDmersGet$Plain$Response(params?: ApiDocumentMyDmersGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DmerDocument>>> {
    return apiDocumentMyDmersGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentMyDmersGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentMyDmersGet$Plain(params?: ApiDocumentMyDmersGet$Plain$Params, context?: HttpContext): Observable<Array<DmerDocument>> {
    return this.apiDocumentMyDmersGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<DmerDocument>>): Array<DmerDocument> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentMyDmersGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentMyDmersGet$Json$Response(params?: ApiDocumentMyDmersGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<DmerDocument>>> {
    return apiDocumentMyDmersGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentMyDmersGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentMyDmersGet$Json(params?: ApiDocumentMyDmersGet$Json$Params, context?: HttpContext): Observable<Array<DmerDocument>> {
    return this.apiDocumentMyDmersGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<DmerDocument>>): Array<DmerDocument> => r.body)
    );
  }

  /** Path part for operation `apiDocumentDriverIdAllDocumentsGet()` */
  static readonly ApiDocumentDriverIdAllDocumentsGetPath = '/api/Document/{driverId}/AllDocuments';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentDriverIdAllDocumentsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentDriverIdAllDocumentsGet$Plain$Response(params: ApiDocumentDriverIdAllDocumentsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDocument>>> {
    return apiDocumentDriverIdAllDocumentsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentDriverIdAllDocumentsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentDriverIdAllDocumentsGet$Plain(params: ApiDocumentDriverIdAllDocumentsGet$Plain$Params, context?: HttpContext): Observable<Array<CaseDocument>> {
    return this.apiDocumentDriverIdAllDocumentsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<CaseDocument>>): Array<CaseDocument> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentDriverIdAllDocumentsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentDriverIdAllDocumentsGet$Json$Response(params: ApiDocumentDriverIdAllDocumentsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<CaseDocument>>> {
    return apiDocumentDriverIdAllDocumentsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentDriverIdAllDocumentsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentDriverIdAllDocumentsGet$Json(params: ApiDocumentDriverIdAllDocumentsGet$Json$Params, context?: HttpContext): Observable<Array<CaseDocument>> {
    return this.apiDocumentDriverIdAllDocumentsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<CaseDocument>>): Array<CaseDocument> => r.body)
    );
  }

  /** Path part for operation `apiDocumentClaimDmerPost()` */
  static readonly ApiDocumentClaimDmerPostPath = '/api/Document/claimDmer';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentClaimDmerPost$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentClaimDmerPost$Plain$Response(params?: ApiDocumentClaimDmerPost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocument>> {
    return apiDocumentClaimDmerPost$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentClaimDmerPost$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentClaimDmerPost$Plain(params?: ApiDocumentClaimDmerPost$Plain$Params, context?: HttpContext): Observable<CaseDocument> {
    return this.apiDocumentClaimDmerPost$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocument>): CaseDocument => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentClaimDmerPost$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentClaimDmerPost$Json$Response(params?: ApiDocumentClaimDmerPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocument>> {
    return apiDocumentClaimDmerPost$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentClaimDmerPost$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentClaimDmerPost$Json(params?: ApiDocumentClaimDmerPost$Json$Params, context?: HttpContext): Observable<CaseDocument> {
    return this.apiDocumentClaimDmerPost$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocument>): CaseDocument => r.body)
    );
  }

  /** Path part for operation `apiDocumentUnclaimDmerPost()` */
  static readonly ApiDocumentUnclaimDmerPostPath = '/api/Document/unclaimDmer';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentUnclaimDmerPost$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentUnclaimDmerPost$Plain$Response(params?: ApiDocumentUnclaimDmerPost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocument>> {
    return apiDocumentUnclaimDmerPost$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentUnclaimDmerPost$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentUnclaimDmerPost$Plain(params?: ApiDocumentUnclaimDmerPost$Plain$Params, context?: HttpContext): Observable<CaseDocument> {
    return this.apiDocumentUnclaimDmerPost$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocument>): CaseDocument => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiDocumentUnclaimDmerPost$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentUnclaimDmerPost$Json$Response(params?: ApiDocumentUnclaimDmerPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<CaseDocument>> {
    return apiDocumentUnclaimDmerPost$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiDocumentUnclaimDmerPost$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiDocumentUnclaimDmerPost$Json(params?: ApiDocumentUnclaimDmerPost$Json$Params, context?: HttpContext): Observable<CaseDocument> {
    return this.apiDocumentUnclaimDmerPost$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<CaseDocument>): CaseDocument => r.body)
    );
  }

}
