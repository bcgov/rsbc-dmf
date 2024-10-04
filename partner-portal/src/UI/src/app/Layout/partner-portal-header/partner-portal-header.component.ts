import { CUSTOM_ELEMENTS_SCHEMA, Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '@app/features/auth/services/auth.service';

@Component({
  selector: 'app-partner-portal-header',
  standalone: true,
  imports: [
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    RouterLink,
    RouterLinkActive,
  ],
  templateUrl: './partner-portal-header.component.html',
  styleUrl: './partner-portal-header.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],

})
export class PartnerPortalHeaderComponent {
  constructor(private authService: AuthService) {}

  logOut() {
    this.authService.logout(window.location.href);
  }
}
