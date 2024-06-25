import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-partner-portal-nav-menu',
  standalone: true,
  imports: [MatToolbarModule, RouterLink],
  templateUrl: './partner-portal-nav-menu.component.html',
  styleUrl: './partner-portal-nav-menu.component.scss',
})
export class PartnerPortalNavMenuComponent {}
