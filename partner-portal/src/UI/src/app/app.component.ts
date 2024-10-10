import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor } from '@angular/common';
import { PartnerPortalFooterComponent } from './Layout/partner-portal-footer/partner-portal-footer.component';
import { PartnerPortalHeaderComponent } from './Layout/partner-portal-header/partner-portal-header.component';
import { PartnerPortalNavMenuComponent } from './Layout/partner-portal-nav-menu/partner-portal-nav-menu.component';
import { RouterOutlet } from '@angular/router';

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
export class AppComponent implements OnInit
{
  ngOnInit() {
      console.info('AppComponent initialization completed.');
  }
}
