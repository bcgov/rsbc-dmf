import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { PartnerPortalFooterComponent } from './Layout/partner-portal-footer/partner-portal-footer.component';
import { PartnerPortalHeaderComponent } from './Layout/partner-portal-header/partner-portal-header.component';
import { PartnerPortalNavMenuComponent } from './Layout/partner-portal-nav-menu/partner-portal-nav-menu.component';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './features/auth/services/auth.service';
import { IdentityProvider } from './features/auth/enums/identity-provider.enum';
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: true,
  imports: [
    NgIf,
    NgFor,
    PartnerPortalFooterComponent,
    PartnerPortalHeaderComponent,
    PartnerPortalNavMenuComponent,
    RouterOutlet,
  ],
})
export class AppComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];

  constructor(private authService: AuthService, private http: HttpClient) {}

  ngOnInit() {

    try {
      console.info('AppComponent initializing...');
      //attempt to log in
      this.authService.isLoggedIn().subscribe((isLoggedIn:boolean) => {
        console.info('Are you logged in?', isLoggedIn);
        if (!isLoggedIn) {
          console.info('Redirect to login page');
          this.authService.login({
            idpHint: IdentityProvider.BCSC,
            // TODO add medical-portal scope and move this to api/Config
            scope: 'openid profile email',
          });
        } else {
          // for spinner status, this will likely change when the keycloak auth lifecycle events are refactored
          //this.isLoading = false;
        }
      });
    } catch (e) {
      console.error(e);
      throw e;
    } finally {
      console.info('AppComponent initialization completed.');
    }

    this.getForecasts();
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('/api/WeatherForecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      },
    );
  }

  title = 'Partner Portal';
}
