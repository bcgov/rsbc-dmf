/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { EmailUpdate } from '../models/email-update';
import { PractitionerBridge } from '../models/practitioner-bridge';
import { UserProfile } from '../models/user-profile';

@Injectable({
  providedIn: 'root',
})
export class ProfileService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation apiProfileCurrentGet
   */
  static readonly ApiProfileCurrentGetPath = '/api/Profile/current';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileCurrentGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Plain$Response(params?: {
  }): Observable<StrictHttpResponse<UserProfile>> {

    const rb = new RequestBuilder(this.rootUrl, ProfileService.ApiProfileCurrentGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<UserProfile>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiProfileCurrentGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Plain(params?: {
  }): Observable<UserProfile> {

    return this.apiProfileCurrentGet$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<UserProfile>) => r.body as UserProfile)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileCurrentGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Json$Response(params?: {
  }): Observable<StrictHttpResponse<UserProfile>> {

    const rb = new RequestBuilder(this.rootUrl, ProfileService.ApiProfileCurrentGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<UserProfile>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiProfileCurrentGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Json(params?: {
  }): Observable<UserProfile> {

    return this.apiProfileCurrentGet$Json$Response(params).pipe(
      map((r: StrictHttpResponse<UserProfile>) => r.body as UserProfile)
    );
  }

  /**
   * Path part for operation apiProfileEmailPut
   */
  static readonly ApiProfileEmailPutPath = '/api/Profile/email';

  /**
   * set the user's profile email.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileEmailPut()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfileEmailPut$Response(params?: {
    body?: EmailUpdate
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProfileService.ApiProfileEmailPutPath, 'put');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * set the user's profile email.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiProfileEmailPut$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfileEmailPut(params?: {
    body?: EmailUpdate
  }): Observable<void> {

    return this.apiProfileEmailPut$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation apiProfilePractitionerRolesGet
   */
  static readonly ApiProfilePractitionerRolesGetPath = '/api/Profile/practitionerRoles';

  /**
   * Get the current practitioner roles.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfilePractitionerRolesGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfilePractitionerRolesGet$Response(params?: {
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProfileService.ApiProfilePractitionerRolesGetPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * Get the current practitioner roles.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiProfilePractitionerRolesGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfilePractitionerRolesGet(params?: {
  }): Observable<void> {

    return this.apiProfilePractitionerRolesGet$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation apiProfilePractitionerRolesPut
   */
  static readonly ApiProfilePractitionerRolesPutPath = '/api/Profile/practitionerRoles';

  /**
   * Add the given practitioner role.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfilePractitionerRolesPut()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfilePractitionerRolesPut$Response(params?: {
    body?: Array<PractitionerBridge>
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProfileService.ApiProfilePractitionerRolesPutPath, 'put');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * Add the given practitioner role.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiProfilePractitionerRolesPut$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfilePractitionerRolesPut(params?: {
    body?: Array<PractitionerBridge>
  }): Observable<void> {

    return this.apiProfilePractitionerRolesPut$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation apiProfilePractitionerRolesDelete
   */
  static readonly ApiProfilePractitionerRolesDeletePath = '/api/Profile/practitionerRoles';

  /**
   * Remove the given practioner role.
   *
   *
   *
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfilePractitionerRolesDelete()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfilePractitionerRolesDelete$Response(params?: {
    body?: Array<PractitionerBridge>
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProfileService.ApiProfilePractitionerRolesDeletePath, 'delete');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * Remove the given practioner role.
   *
   *
   *
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `apiProfilePractitionerRolesDelete$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfilePractitionerRolesDelete(params?: {
    body?: Array<PractitionerBridge>
  }): Observable<void> {

    return this.apiProfilePractitionerRolesDelete$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
