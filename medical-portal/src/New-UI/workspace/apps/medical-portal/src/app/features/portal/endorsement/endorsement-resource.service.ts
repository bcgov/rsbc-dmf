/* eslint-disable @typescript-eslint/explicit-member-accessibility */
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable, catchError, of } from 'rxjs';

import { ApiHttpClient } from '@app/core/resources/api-http-client.service';

@Injectable({
  providedIn: 'root',
})
export class EndorsementResource {
  public constructor(private apiResource: ApiHttpClient) {}

  public getEndorsements(hpdid: string): Observable<EndorsementList[]> {
    return this.apiResource
      .get<EndorsementList[]>(
        `Contacts/${hpdid.replace('@bcsc', '')}/endorsements`
      )
      .pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === HttpStatusCode.NotFound) {
            return of([]);
          }

          throw error; //catch and handle this in prod!
        })
      );
  }
}
export interface EndorsementList {
  hpdid: string;
  licences: Licenses[];
  contact: Contact;
}
export class Contact {
  contactId: string;
  firstName: string;
  lastName: string;
  email: string;
  birthDate: string;
  role: string;

  constructor() {
    this.contactId = '';
    this.email = '';
    this.birthDate = '';
    this.firstName = '';
    this.lastName = '';
    this.role = '';
  }
}

export interface Licenses {
  identifierType: string;
  statusCode: string;
  statusReasonCode: string;
}
