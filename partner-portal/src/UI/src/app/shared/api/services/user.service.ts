/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { apiUserGetContactRolesGet$Json } from '../fn/user/api-user-get-contact-roles-get-json';
import { ApiUserGetContactRolesGet$Json$Params } from '../fn/user/api-user-get-contact-roles-get-json';
import { apiUserGetContactRolesGet$Plain } from '../fn/user/api-user-get-contact-roles-get-plain';
import { ApiUserGetContactRolesGet$Plain$Params } from '../fn/user/api-user-get-contact-roles-get-plain';
import { apiUserGetCurrentLoginDetailsGet$Json } from '../fn/user/api-user-get-current-login-details-get-json';
import { ApiUserGetCurrentLoginDetailsGet$Json$Params } from '../fn/user/api-user-get-current-login-details-get-json';
import { apiUserGetCurrentLoginDetailsGet$Plain } from '../fn/user/api-user-get-current-login-details-get-plain';
import { ApiUserGetCurrentLoginDetailsGet$Plain$Params } from '../fn/user/api-user-get-current-login-details-get-plain';
import { apiUserGetUsersPost$Json } from '../fn/user/api-user-get-users-post-json';
import { ApiUserGetUsersPost$Json$Params } from '../fn/user/api-user-get-users-post-json';
import { apiUserGetUsersPost$Plain } from '../fn/user/api-user-get-users-post-plain';
import { ApiUserGetUsersPost$Plain$Params } from '../fn/user/api-user-get-users-post-plain';
import { apiUserUpdateContactRolesPost } from '../fn/user/api-user-update-contact-roles-post';
import { ApiUserUpdateContactRolesPost$Params } from '../fn/user/api-user-update-contact-roles-post';
import { apiUserUpdateUserPost$Json } from '../fn/user/api-user-update-user-post-json';
import { ApiUserUpdateUserPost$Json$Params } from '../fn/user/api-user-update-user-post-json';
import { apiUserUpdateUserPost$Plain } from '../fn/user/api-user-update-user-post-plain';
import { ApiUserUpdateUserPost$Plain$Params } from '../fn/user/api-user-update-user-post-plain';
import { User } from '../models/user';
import { UserRole } from '../models/user-role';

@Injectable({ providedIn: 'root' })
export class UserService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `apiUserGetUsersPost()` */
  static readonly ApiUserGetUsersPostPath = '/api/User/getUsers';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserGetUsersPost$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserGetUsersPost$Plain$Response(params?: ApiUserGetUsersPost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<User>>> {
    return apiUserGetUsersPost$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserGetUsersPost$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserGetUsersPost$Plain(params?: ApiUserGetUsersPost$Plain$Params, context?: HttpContext): Observable<Array<User>> {
    return this.apiUserGetUsersPost$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<User>>): Array<User> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserGetUsersPost$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserGetUsersPost$Json$Response(params?: ApiUserGetUsersPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<User>>> {
    return apiUserGetUsersPost$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserGetUsersPost$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserGetUsersPost$Json(params?: ApiUserGetUsersPost$Json$Params, context?: HttpContext): Observable<Array<User>> {
    return this.apiUserGetUsersPost$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<User>>): Array<User> => r.body)
    );
  }

  /** Path part for operation `apiUserUpdateUserPost()` */
  static readonly ApiUserUpdateUserPostPath = '/api/User/updateUser';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserUpdateUserPost$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserUpdateUserPost$Plain$Response(params?: ApiUserUpdateUserPost$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<User>>> {
    return apiUserUpdateUserPost$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserUpdateUserPost$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserUpdateUserPost$Plain(params?: ApiUserUpdateUserPost$Plain$Params, context?: HttpContext): Observable<Array<User>> {
    return this.apiUserUpdateUserPost$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<User>>): Array<User> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserUpdateUserPost$Json()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserUpdateUserPost$Json$Response(params?: ApiUserUpdateUserPost$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<User>>> {
    return apiUserUpdateUserPost$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserUpdateUserPost$Json$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserUpdateUserPost$Json(params?: ApiUserUpdateUserPost$Json$Params, context?: HttpContext): Observable<Array<User>> {
    return this.apiUserUpdateUserPost$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<User>>): Array<User> => r.body)
    );
  }

  /** Path part for operation `apiUserGetContactRolesGet()` */
  static readonly ApiUserGetContactRolesGetPath = '/api/User/getContactRoles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserGetContactRolesGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGetContactRolesGet$Plain$Response(params?: ApiUserGetContactRolesGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<UserRole>>> {
    return apiUserGetContactRolesGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserGetContactRolesGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGetContactRolesGet$Plain(params?: ApiUserGetContactRolesGet$Plain$Params, context?: HttpContext): Observable<Array<UserRole>> {
    return this.apiUserGetContactRolesGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<UserRole>>): Array<UserRole> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserGetContactRolesGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGetContactRolesGet$Json$Response(params?: ApiUserGetContactRolesGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<UserRole>>> {
    return apiUserGetContactRolesGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserGetContactRolesGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGetContactRolesGet$Json(params?: ApiUserGetContactRolesGet$Json$Params, context?: HttpContext): Observable<Array<UserRole>> {
    return this.apiUserGetContactRolesGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<UserRole>>): Array<UserRole> => r.body)
    );
  }

  /** Path part for operation `apiUserUpdateContactRolesPost()` */
  static readonly ApiUserUpdateContactRolesPostPath = '/api/User/updateContactRoles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserUpdateContactRolesPost()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserUpdateContactRolesPost$Response(params?: ApiUserUpdateContactRolesPost$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return apiUserUpdateContactRolesPost(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserUpdateContactRolesPost$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  apiUserUpdateContactRolesPost(params?: ApiUserUpdateContactRolesPost$Params, context?: HttpContext): Observable<void> {
    return this.apiUserUpdateContactRolesPost$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `apiUserGetCurrentLoginDetailsGet()` */
  static readonly ApiUserGetCurrentLoginDetailsGetPath = '/api/User/getCurrentLoginDetails';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserGetCurrentLoginDetailsGet$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGetCurrentLoginDetailsGet$Plain$Response(params?: ApiUserGetCurrentLoginDetailsGet$Plain$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<string>>> {
    return apiUserGetCurrentLoginDetailsGet$Plain(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserGetCurrentLoginDetailsGet$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGetCurrentLoginDetailsGet$Plain(params?: ApiUserGetCurrentLoginDetailsGet$Plain$Params, context?: HttpContext): Observable<Array<string>> {
    return this.apiUserGetCurrentLoginDetailsGet$Plain$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<string>>): Array<string> => r.body)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `apiUserGetCurrentLoginDetailsGet$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGetCurrentLoginDetailsGet$Json$Response(params?: ApiUserGetCurrentLoginDetailsGet$Json$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<string>>> {
    return apiUserGetCurrentLoginDetailsGet$Json(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `apiUserGetCurrentLoginDetailsGet$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  apiUserGetCurrentLoginDetailsGet$Json(params?: ApiUserGetCurrentLoginDetailsGet$Json$Params, context?: HttpContext): Observable<Array<string>> {
    return this.apiUserGetCurrentLoginDetailsGet$Json$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<string>>): Array<string> => r.body)
    );
  }

}
