import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  private get token(): null | string { return window.sessionStorage.getItem('auth:token') || null; }
  private set token(v: null | string) {
    if (v != null) {
      window.sessionStorage.setItem('auth:token', v);
    } else {
      window.sessionStorage.removeItem('auth:token');
    }
  }

  constructor(private router: Router) {
  }

  public login(returnUrl: string = '/'): Observable<boolean> {
    console.debug('login');
    if (this.token !== null) {
      return of(true);
    }
    this.router.navigate(['login']);
    return of(false);
  }
  public logout(): Observable<boolean> {
    console.log('logout');
    this.token = null;
    this.router.navigate(['login']);
    return of(false);
  }

  public isLoggedIn(): boolean {
    return this.token !== null;
  }

  public setLoggedIn(): void {
    this.token = 'token';
    this.router.navigate(['/']);
  }

}
