import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Resolve, Router } from '@angular/router';

import { Observable, catchError, exhaustMap, of, throwError } from 'rxjs';

import { RootRoutes } from '@bcgov/shared/ui';

import { ShellRoutes } from '@app/features/shell/shell.routes';

import { LoggerService } from '../services/logger.service';
import { ContactResource } from './contact-resource.service';
import { ContactService } from './contact.service';

/**
 * @description
 * Gets a Contact from the API based on the access token, and
 * if not found creates the resource before setting the Contact
 * identifier in a singleton service.
 *
 * WARNING: Should be located on or under the route config
 * containing guard(s) that manage access, otherwise will
 * redirect to access denied when unauthenticated.
 */
@Injectable({
  providedIn: 'root',
})
export class ContactResolver implements Resolve<string | null> {
  public constructor(
    private router: Router,
    private contactResource: ContactResource,
    private contactService: ContactService,
    private logger: LoggerService
  ) {}

  public resolve(): Observable<string | null> {
    return this.contactResource.firstOrCreate().pipe(
      exhaustMap((contactId: string | null) =>
        contactId
          ? of((this.contactService.contactId = contactId))
          : throwError(() => new Error('Contact could not be found or created'))
      ),
      catchError((error: HttpErrorResponse) => {
        this.logger.error(error.message);
        console.log(error.status);
        error.status === 403
          ? this.router.navigateByUrl(RootRoutes.DENIED)
          : this.router.navigateByUrl(ShellRoutes.SUPPORT_ERROR_PAGE);
        return of(null);
      })
    );
  }
}
