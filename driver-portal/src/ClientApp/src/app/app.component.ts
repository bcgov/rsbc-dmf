/* eslint-disable @angular-eslint/no-empty-lifecycle-method */
import { CUSTOM_ELEMENTS_SCHEMA, Component, Inject, OnInit, Optional } from '@angular/core';
import { LoginService } from './shared/services/login.service';
import { ConfigurationService } from './shared/services/configuration.service';
import { Router, RouterOutlet } from '@angular/router';
import { APP_BASE_HREF, NgIf } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { FooterComponent, HeaderComponent, NavMenuComponent } from '@shared/core-ui'
import { ApplicationVersionInfoService } from './shared/api/services';
import { ApplicationVersionInfo } from './shared/api/models';
import { NgxSpinnerComponent } from 'ngx-spinner';


@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: true,
    imports: [
        NgIf,
        RouterOutlet,
        HeaderComponent,
        FooterComponent,
        NavMenuComponent,
        NgxSpinnerComponent
    ],
    schemas:[CUSTOM_ELEMENTS_SCHEMA]
})
export class AppComponent implements OnInit {
  public isLoading = true;
  public profileName : string = ''; 
  public versionInfo : ApplicationVersionInfo | null = null;

  constructor(
    @Inject(APP_BASE_HREF) public baseHref: string,
    public loginService: LoginService,
    private configService: ConfigurationService,
    private router: Router,
    @Optional() private versionInfoDataService: ApplicationVersionInfoService
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

      // Get user profile name and initials
      const profile = this.loginService.userProfile;

      if (profile) {
        const firstName = profile.firstName;
        const lastName = profile.lastName;
        this.profileName = firstName + ' ' + lastName;
      }

      // Get Version info on footer
      if (this.versionInfoDataService !== null)
      {
        this.versionInfoDataService.apiApplicationVersionInfoGet$Json()
          .subscribe((versionInfo: ApplicationVersionInfo) => {
            this.versionInfo = versionInfo;
            
          });
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

  public onLogout():void{
    this.loginService.logout();
  }
}
