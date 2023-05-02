/* eslint-disable @typescript-eslint/explicit-member-accessibility */
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  Router,
  RouterStateSnapshot,
} from '@angular/router';

import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class EndorsementResolver implements Resolve<boolean> {
  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return of(true);
  }
}
