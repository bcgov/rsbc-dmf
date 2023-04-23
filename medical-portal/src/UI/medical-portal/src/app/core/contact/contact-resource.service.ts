import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable, catchError, exhaustMap, map, of, throwError } from 'rxjs';

import { ApiHttpClient } from '@app/core/resources/api-http-client.service';
import { User } from '@app/features/auth/models/user.model';
import { AuthorizedUserService } from '@app/features/auth/services/authorized-user.service';

import { ContactCreate } from './contact-create.model';

@Injectable({
  providedIn: 'root',
})
export class ContactResource {
  public constructor(
    private apiResource: ApiHttpClient,
    private authorizedUserService: AuthorizedUserService
  ) {}

  /**
   * @description
   * Get a contact ID based on the access token user ID, and
   * create a contact if one does not already exist.
   */
  public firstOrCreate(): Observable<string | null> {
    return this.authorizedUserService.user$.pipe(
      exhaustMap((user: User) =>
        user
          ? this.getCredentials().pipe(
              map((contactId: string | null) => contactId ?? user)
            )
          : throwError(
              () =>
                new Error(
                  'Not authenticated or access token could not be parsed'
                )
            )
      ),
      exhaustMap((contactIdOrUser: string | User | null) =>
        typeof contactIdOrUser === 'string' || !contactIdOrUser
          ? of(contactIdOrUser)
          : this.createContact(contactIdOrUser)
      )
    );
  }

  /**
   * @description
   * Discovery endpoint for checking the existence of a Contact
   * based on a UserId, which provides a ContactId in response.
   */
  private getCredentials(): Observable<string | null> {
    return this.apiResource.get<{ contactId: string }[]>('credentials').pipe(
      map((parties: { contactId: string }[]) =>
        parties?.length ? parties.shift()?.contactId ?? null : null
      ),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 400) {
          return of(null);
        }

        return throwError(
          () =>
            new Error(
              `Error occurred attempting to retrieve Contact Credentials`
            )
        );
      })
    );
  }

  /**
   * @description
   * Create a new contact from information provided from the
   * access token.
   */
  private createContact(
    contactCreate: ContactCreate
  ): Observable<string | null> {
    return this.apiResource.post<string>('parties', contactCreate).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 400) {
          return of(null);
        }

        return throwError(() => new Error('Contact could not be created'));
      })
    );
  }
}
