import { Component, OnInit } from '@angular/core';
import { LoginService } from './login.service';
import { ConfigurationService } from './shared/services/configuration.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})

export class AppComponent implements OnInit {

  constructor(
    private loginService: LoginService,
    private configService: ConfigurationService
  ) { }

  public ngOnInit(): void {
    this.configService.load().subscribe(
      (result) => { },
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
