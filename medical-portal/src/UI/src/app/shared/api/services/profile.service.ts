/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiProfileCurrentGet$Json } from '../fn/profile/api-profile-current-get-json';
import { ApiProfileCurrentGet$Json$Params } from '../fn/profile/api-profile-current-get-json';
import { apiProfileCurrentGet$Plain } from '../fn/profile/api-profile-current-get-plain';
import { ApiProfileCurrentGet$Plain$Params } from '../fn/profile/api-profile-current-get-plain';
import { apiProfileEmailPut } from '../fn/profile/api-profile-email-put';
import { ApiProfileEmailPut$Params } from '../fn/profile/api-profile-email-put';
import { apiProfilePractitionerRolesDelete } from '../fn/profile/api-profile-practitioner-roles-delete';
import { ApiProfilePractitionerRolesDelete$Params } from '../fn/profile/api-profile-practitioner-roles-delete';
import { apiProfilePractitionerRolesGet } from '../fn/profile/api-profile-practitioner-roles-get';
import { ApiProfilePractitionerRolesGet$Params } from '../fn/profile/api-profile-practitioner-roles-get';
import { apiProfilePractitionerRolesPut } from '../fn/profile/api-profile-practitioner-roles-put';
import { ApiProfilePractitionerRolesPut$Params } from '../fn/profile/api-profile-practitioner-roles-put';
import { UserProfile } from '../models/user-profile';

@Injectable({ providedIn: 'root' })
export class ProfileService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiProfileCurrentGet()` */
  static readonly ApiProfileCurrentGetPath = '/api/Profile/current';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileCurrentGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Plain$Response(params?: ApiProfileCurrentGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<UserProfile>> {
    return apiProfileCurrentGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfileCurrentGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Plain(params?: ApiProfileCurrentGet$Plain$Params, context?: HttpContext): Observable<UserProfile> {
    return this.apiProfileCurrentGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<UserProfile>): UserProfile => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileCurrentGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Json$Response(params?: ApiProfileCurrentGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<UserProfile>> {
    return apiProfileCurrentGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfileCurrentGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfileCurrentGet$Json(params?: ApiProfileCurrentGet$Json$Params, context?: HttpContext): Observable<UserProfile> {
    return this.apiProfileCurrentGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<UserProfile>): UserProfile => r.body)
    );
  }

  /** Path part for operation `apiProfileEmailPut()` */
  static readonly ApiProfileEmailPutPath = '/api/Profile/email';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfileEmailPut()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfileEmailPut$Response(params?: ApiProfileEmailPut$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiProfileEmailPut(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfileEmailPut$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfileEmailPut(params?: ApiProfileEmailPut$Params, context?: HttpContext): Observable<void> {
    return this.apiProfileEmailPut$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `apiProfilePractitionerRolesGet()` */
  static readonly ApiProfilePractitionerRolesGetPath = '/api/Profile/practitionerRoles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfilePractitionerRolesGet()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfilePractitionerRolesGet$Response(params?: ApiProfilePractitionerRolesGet$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiProfilePractitionerRolesGet(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfilePractitionerRolesGet$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiProfilePractitionerRolesGet(params?: ApiProfilePractitionerRolesGet$Params, context?: HttpContext): Observable<void> {
    return this.apiProfilePractitionerRolesGet$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `apiProfilePractitionerRolesPut()` */
  static readonly ApiProfilePractitionerRolesPutPath = '/api/Profile/practitionerRoles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfilePractitionerRolesPut()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfilePractitionerRolesPut$Response(params?: ApiProfilePractitionerRolesPut$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiProfilePractitionerRolesPut(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfilePractitionerRolesPut$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfilePractitionerRolesPut(params?: ApiProfilePractitionerRolesPut$Params, context?: HttpContext): Observable<void> {
    return this.apiProfilePractitionerRolesPut$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `apiProfilePractitionerRolesDelete()` */
  static readonly ApiProfilePractitionerRolesDeletePath = '/api/Profile/practitionerRoles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiProfilePractitionerRolesDelete()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfilePractitionerRolesDelete$Response(params?: ApiProfilePractitionerRolesDelete$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiProfilePractitionerRolesDelete(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiProfilePractitionerRolesDelete$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiProfilePractitionerRolesDelete(params?: ApiProfilePractitionerRolesDelete$Params, context?: HttpContext): Observable<void> {
    return this.apiProfilePractitionerRolesDelete$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

}
