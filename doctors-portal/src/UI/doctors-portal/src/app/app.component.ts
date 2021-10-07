import { Component, Inject, OnInit } from '@angular/core';
import { LoginService } from './shared/services/login.service';
import { ConfigurationService } from './shared/services/configuration.service';
import { Router } from '@angular/router';
import { APP_BASE_HREF } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})

export class AppComponent implements OnInit {

  public isLoading: boolean = true;

  constructor(
    @Inject(APP_BASE_HREF) public baseHref: string,
    private loginService: LoginService,
    private configService: ConfigurationService,
    private router: Router
  ) { }

  public async ngOnInit(): Promise<void> {
    try {
      //load the configuration from the server
      await this.configService.load().toPromise();
      //attempt to log in
      let nextRoute = await this.loginService.login(location.pathname.substring(1) || 'dashboard').toPromise();

      //get the user's profile
      await this.loginService.getUserProfile().toPromise();

      //determine the next route
      nextRoute = '/' + decodeURIComponent(nextRoute);

      // remove base path.
      if (this.baseHref && nextRoute.substring(0, this.baseHref.length) === this.baseHref) {
        nextRoute = nextRoute.substring(this.baseHref.length);
      }

      this.router.navigate([nextRoute]).then(() => this.isLoading = false)
    } catch (e) {
      console.error(e);
      throw e;
    }
  }

  public showNavigation(): boolean {
    return this.loginService.isLoggedIn();
  }

  public showProfile(): boolean {
    return this.loginService.isLoggedIn();
  }
}
