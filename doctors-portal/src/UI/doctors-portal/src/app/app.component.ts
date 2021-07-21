import { Component, OnInit } from '@angular/core';
import { LoginService } from './shared/services/login.service';
import { ConfigurationService } from './shared/services/configuration.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})

export class AppComponent implements OnInit {

  public isLoading: boolean = true;

  constructor(
    private loginService: LoginService,
    private configService: ConfigurationService,
    private router: Router
  ) { }

  public ngOnInit(): void {
    this.configService.load().subscribe(
      (result) => {
        this.loginService.login(location.pathname.substring(1) || 'dashboard')
          .subscribe(nextRoute => {
            console.debug('logged in', nextRoute);
            //if (nextRoute) {
            this.router.navigate([decodeURIComponent(nextRoute)]).then(() => this.isLoading = false)
            //}
          }, error => console.error(error));
      },
      (error) => {
        //TODO: navigate to service unavailable page
        console.error('failed to load configuration from the server');
      }
    );
  }

  public showNavigation(): boolean {
    return this.loginService.isLoggedIn();
  }

  public showProfile(): boolean {
    return this.loginService.isLoggedIn();
  }
}
