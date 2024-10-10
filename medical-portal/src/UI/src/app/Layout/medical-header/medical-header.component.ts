import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '@shared/core-ui';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    RouterLink,
    RouterLinkActive,
  ],
  templateUrl: './medical-header.component.html',
  styleUrl: './medical-header.component.scss',
})
export class MedicalHeaderComponent {
  constructor(private authService: AuthService) {}

  logOut() {
    this.authService.logout(window.location.href);
  }
}
