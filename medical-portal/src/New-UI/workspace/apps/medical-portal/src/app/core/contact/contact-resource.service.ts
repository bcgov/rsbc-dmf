import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { Observable, catchError, exhaustMap, map, of, throwError } from 'rxjs';

import { ResourceUtilsService } from '@bcgov/shared/data-access';
import { RootRoutes } from '@bcgov/shared/ui';

import { ApiHttpClient } from '@app/core/resources/api-http-client.service';
import { User } from '@app/features/auth/models/user.model';
import { AuthorizedUserService } from '@app/features/auth/services/authorized-user.service';
import { ShellRoutes } from '@app/features/shell/shell.routes';

import { ContactCreate } from './contact-create.model';

@Injectable({
  providedIn: 'root',
})
export class ContactResource {
  public constructor(
    private apiResource: ApiHttpClient,
    private router: Router,
    private resourceUtilsService: ResourceUtilsService,
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
          ? this.getContacts(user.idpId).pipe(
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
  private getContacts(contactId: string): Observable<string | null> {
    const params = this.resourceUtilsService.makeHttpParams({ contactId });
    return this.apiResource.get<{ id: string }[]>('contacts', { params }).pipe(
      map((contacts: { id: string }[]) =>
        contacts?.length ? contacts.shift()?.id ?? null : null
      ),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 400) {
          return of(null);
        }
        if (error.status === 403) {
          this.router.navigateByUrl(RootRoutes.DENIED);
        }

        return throwError(
          () =>
            new Error(
              `Error occurred attempting to retrieve Contact with user ID ${contactId}`
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
    return this.apiResource
      .post<string>('contacts', contactCreate, { responseType: 'text' })
      .pipe(
        map((response) => {
          const match = response.match(/[\w-]+/);
          return match ? match[0] : null;
        }),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 400) {
            return of(null);
          }

          return throwError(() => new Error('Contact could not be created'));
        })
      );
  }
}
