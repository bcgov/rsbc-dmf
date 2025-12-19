/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiPortalUserGetContactRolesGet$Json } from '../fn/portal-user/api-portal-user-get-contact-roles-get-json';
import { ApiPortalUserGetContactRolesGet$Json$Params } from '../fn/portal-user/api-portal-user-get-contact-roles-get-json';
import { apiPortalUserGetContactRolesGet$Plain } from '../fn/portal-user/api-portal-user-get-contact-roles-get-plain';
import { ApiPortalUserGetContactRolesGet$Plain$Params } from '../fn/portal-user/api-portal-user-get-contact-roles-get-plain';
import { apiPortalUserGetCurrentLoginDetailsGet$Json } from '../fn/portal-user/api-portal-user-get-current-login-details-get-json';
import { ApiPortalUserGetCurrentLoginDetailsGet$Json$Params } from '../fn/portal-user/api-portal-user-get-current-login-details-get-json';
import { apiPortalUserGetCurrentLoginDetailsGet$Plain } from '../fn/portal-user/api-portal-user-get-current-login-details-get-plain';
import { ApiPortalUserGetCurrentLoginDetailsGet$Plain$Params } from '../fn/portal-user/api-portal-user-get-current-login-details-get-plain';
import { apiPortalUserGetUsersPost$Json } from '../fn/portal-user/api-portal-user-get-users-post-json';
import { ApiPortalUserGetUsersPost$Json$Params } from '../fn/portal-user/api-portal-user-get-users-post-json';
import { apiPortalUserGetUsersPost$Plain } from '../fn/portal-user/api-portal-user-get-users-post-plain';
import { ApiPortalUserGetUsersPost$Plain$Params } from '../fn/portal-user/api-portal-user-get-users-post-plain';
import { apiPortalUserUpdateContactRolesPost } from '../fn/portal-user/api-portal-user-update-contact-roles-post';
import { ApiPortalUserUpdateContactRolesPost$Params } from '../fn/portal-user/api-portal-user-update-contact-roles-post';
import { apiPortalUserUpdateUserPost$Json } from '../fn/portal-user/api-portal-user-update-user-post-json';
import { ApiPortalUserUpdateUserPost$Json$Params } from '../fn/portal-user/api-portal-user-update-user-post-json';
import { apiPortalUserUpdateUserPost$Plain } from '../fn/portal-user/api-portal-user-update-user-post-plain';
import { ApiPortalUserUpdateUserPost$Plain$Params } from '../fn/portal-user/api-portal-user-update-user-post-plain';
import { User } from '../models/user';
import { UserRole } from '../models/user-role';

@Injectable({ providedIn: 'root' })
export class PortalUserService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiPortalUserGetUsersPost()` */
  static readonly ApiPortalUserGetUsersPostPath = '/api/PortalUser/getUsers';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserGetUsersPost$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserGetUsersPost$Plain$Response(params?: ApiPortalUserGetUsersPost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<User>>> {
    return apiPortalUserGetUsersPost$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserGetUsersPost$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserGetUsersPost$Plain(params?: ApiPortalUserGetUsersPost$Plain$Params, context?: HttpContext): Observable<Array<User>> {
    return this.apiPortalUserGetUsersPost$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<User>>): Array<User> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserGetUsersPost$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserGetUsersPost$Json$Response(params?: ApiPortalUserGetUsersPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<User>>> {
    return apiPortalUserGetUsersPost$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserGetUsersPost$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserGetUsersPost$Json(params?: ApiPortalUserGetUsersPost$Json$Params, context?: HttpContext): Observable<Array<User>> {
    return this.apiPortalUserGetUsersPost$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<User>>): Array<User> => r.body)
    );
  }

  /** Path part for operation `apiPortalUserUpdateUserPost()` */
  static readonly ApiPortalUserUpdateUserPostPath = '/api/PortalUser/updateUser';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserUpdateUserPost$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserUpdateUserPost$Plain$Response(params?: ApiPortalUserUpdateUserPost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<User>> {
    return apiPortalUserUpdateUserPost$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserUpdateUserPost$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserUpdateUserPost$Plain(params?: ApiPortalUserUpdateUserPost$Plain$Params, context?: HttpContext): Observable<User> {
    return this.apiPortalUserUpdateUserPost$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<User>): User => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserUpdateUserPost$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserUpdateUserPost$Json$Response(params?: ApiPortalUserUpdateUserPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<User>> {
    return apiPortalUserUpdateUserPost$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserUpdateUserPost$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserUpdateUserPost$Json(params?: ApiPortalUserUpdateUserPost$Json$Params, context?: HttpContext): Observable<User> {
    return this.apiPortalUserUpdateUserPost$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<User>): User => r.body)
    );
  }

  /** Path part for operation `apiPortalUserGetContactRolesGet()` */
  static readonly ApiPortalUserGetContactRolesGetPath = '/api/PortalUser/getContactRoles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserGetContactRolesGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPortalUserGetContactRolesGet$Plain$Response(params?: ApiPortalUserGetContactRolesGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<UserRole>>> {
    return apiPortalUserGetContactRolesGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserGetContactRolesGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPortalUserGetContactRolesGet$Plain(params?: ApiPortalUserGetContactRolesGet$Plain$Params, context?: HttpContext): Observable<Array<UserRole>> {
    return this.apiPortalUserGetContactRolesGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<UserRole>>): Array<UserRole> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserGetContactRolesGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPortalUserGetContactRolesGet$Json$Response(params?: ApiPortalUserGetContactRolesGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<UserRole>>> {
    return apiPortalUserGetContactRolesGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserGetContactRolesGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPortalUserGetContactRolesGet$Json(params?: ApiPortalUserGetContactRolesGet$Json$Params, context?: HttpContext): Observable<Array<UserRole>> {
    return this.apiPortalUserGetContactRolesGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<UserRole>>): Array<UserRole> => r.body)
    );
  }

  /** Path part for operation `apiPortalUserUpdateContactRolesPost()` */
  static readonly ApiPortalUserUpdateContactRolesPostPath = '/api/PortalUser/updateContactRoles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserUpdateContactRolesPost()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserUpdateContactRolesPost$Response(params?: ApiPortalUserUpdateContactRolesPost$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiPortalUserUpdateContactRolesPost(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserUpdateContactRolesPost$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiPortalUserUpdateContactRolesPost(params?: ApiPortalUserUpdateContactRolesPost$Params, context?: HttpContext): Observable<void> {
    return this.apiPortalUserUpdateContactRolesPost$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `apiPortalUserGetCurrentLoginDetailsGet()` */
  static readonly ApiPortalUserGetCurrentLoginDetailsGetPath = '/api/PortalUser/getCurrentLoginDetails';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserGetCurrentLoginDetailsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPortalUserGetCurrentLoginDetailsGet$Plain$Response(params?: ApiPortalUserGetCurrentLoginDetailsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<string>>> {
    return apiPortalUserGetCurrentLoginDetailsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserGetCurrentLoginDetailsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPortalUserGetCurrentLoginDetailsGet$Plain(params?: ApiPortalUserGetCurrentLoginDetailsGet$Plain$Params, context?: HttpContext): Observable<Array<string>> {
    return this.apiPortalUserGetCurrentLoginDetailsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<string>>): Array<string> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiPortalUserGetCurrentLoginDetailsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPortalUserGetCurrentLoginDetailsGet$Json$Response(params?: ApiPortalUserGetCurrentLoginDetailsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<string>>> {
    return apiPortalUserGetCurrentLoginDetailsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiPortalUserGetCurrentLoginDetailsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiPortalUserGetCurrentLoginDetailsGet$Json(params?: ApiPortalUserGetCurrentLoginDetailsGet$Json$Params, context?: HttpContext): Observable<Array<string>> {
    return this.apiPortalUserGetCurrentLoginDetailsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<string>>): Array<string> => r.body)
    );
  }

}
