import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { PartnerPortalFooterComponent } from './Layout/partner-portal-footer/partner-portal-footer.component';
import { PartnerPortalHeaderComponent } from './Layout/partner-portal-header/partner-portal-header.component';
import { PartnerPortalNavMenuComponent } from './Layout/partner-portal-nav-menu/partner-portal-nav-menu.component';
import { RouterOutlet } from '@angular/router';
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

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getForecasts();
  }

  getForecasts() {
    this.http.get<WeatherForecast[]>('/weatherforecast').subscribe(
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
