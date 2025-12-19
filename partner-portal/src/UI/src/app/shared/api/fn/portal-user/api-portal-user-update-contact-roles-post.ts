/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { UpdateContactRole } from '../../models/update-contact-role';

export interface ApiPortalUserUpdateContactRolesPost$Params {
      body?: UpdateContactRole
}

export function apiPortalUserUpdateContactRolesPost(http: HttpClient, rootUrl: string, params?: ApiPortalUserUpdateContactRolesPost$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
  const rb = new RequestBuilder(rootUrl, apiPortalUserUpdateContactRolesPost.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: '*/*', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
    })
  );
}

apiPortalUserUpdateContactRolesPost.PATH = '/api/PortalUser/updateContactRoles';
