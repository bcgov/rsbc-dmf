import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  private loggedIn = false;

  constructor(private router: Router) { }

  public Login(returnUrl: string = '/'): Observable<boolean> {
    console.log(this.loggedIn);
    if (this.loggedIn) {
      return of(true);
    }
    this.router.navigate(['login']);
    return of(false);
  }
  public Logout(): Observable<boolean> {
    this.loggedIn = false;
    this.router.navigate(['login']);
    return of(false);
  }

  public isLoggedIn(): boolean {
    return this.loggedIn;
  }

  public setLoggedIn(): void {
    this.loggedIn = true;
    this.router.navigate(['/']);
  }

}
