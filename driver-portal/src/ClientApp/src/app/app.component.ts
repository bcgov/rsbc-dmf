/* eslint-disable @angular-eslint/no-empty-lifecycle-method */
import { Component, Inject, OnInit } from '@angular/core';
import { LoginService } from './shared/services/login.service';
import { ConfigurationService } from './shared/services/configuration.service';
import { Router } from '@angular/router';
import { APP_BASE_HREF } from '@angular/common';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  public isLoading = true;

  constructor(
    @Inject(APP_BASE_HREF) public baseHref: string,
    public loginService: LoginService,
    private configService: ConfigurationService,
    private router: Router
  ) {}

  public async ngOnInit(): Promise<void> {
    try {
      //load the configuration from the server
      await firstValueFrom(this.configService.load());

      //attempt to log in
      let nextRoute = await firstValueFrom(
        this.loginService.login(location.pathname.substring(1) || 'dashboard')
      );

      //get the user's profile

      const driver = await firstValueFrom(this.loginService.getUserProfile());
     
      if (!this.loginService.isLoggedIn()) {
        return;
      }

      if (driver.driverId) {
        //if the user is logged in, redirect to the dashboard
        nextRoute = 'dashboard';
      } else {
        nextRoute = 'userRegistration';
      }

      //determine the next route
      nextRoute = '/' + decodeURIComponent(nextRoute);

      // remove base path.
      if (
        this.baseHref &&
        nextRoute.substring(0, this.baseHref.length) === this.baseHref
      ) {
        nextRoute = nextRoute.substring(this.baseHref.length);
      }

      this.router.navigate([nextRoute]).then(() => (this.isLoading = false));
    } catch (e) {
      console.error(e);
      // this.router
      //   .navigate(['userRegistration'])
      //   .then(() => (this.isLoading = false));
      console.error(e);
      throw e;
    }
  }

  public showNavigation(): boolean {
    return this.loginService.isLoggedIn();
  }

  // public showProfile(): boolean {
  //   return this.loginService.isLoggedIn();
  // }
}
