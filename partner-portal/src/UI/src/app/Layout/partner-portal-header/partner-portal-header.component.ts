import { CUSTOM_ELEMENTS_SCHEMA, Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '@shared/core-ui';
import { ProfileService } from '@app/shared/api/services';

@Component({
  selector: 'app-partner-portal-header',
  standalone: true,
  imports: [
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './partner-portal-header.component.html',
  styleUrl: './partner-portal-header.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],

})
export class PartnerPortalHeaderComponent implements OnInit {
  userName = '';

  constructor(
    private authService: AuthService,
    private profileService: ProfileService,
  ) {}

  ngOnInit(): void {
    this.profileService.apiProfileCurrentGet$Json().subscribe({
      next: (profile) => {
        const firstName = profile.firstName?.trim() ?? '';
        const lastName = profile.lastName?.trim() ?? '';
        this.userName = `${firstName} ${lastName}`.trim();
      },
      error: (error) => {
        console.error('Error loading current user profile', error);
      },
    });
  }

  logOut() {
    this.authService.logout(window.location.href);
  }
}
