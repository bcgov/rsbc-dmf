import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { UserService } from '@app/shared/services/user.service';
import { AdminAuthGuard } from '@app/modules/admin/admin.guard';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-partner-portal-nav-menu',
  standalone: true,
  imports: [MatToolbarModule, RouterLink, CommonModule],
  templateUrl: './partner-portal-nav-menu.component.html',
  styleUrl: './partner-portal-nav-menu.component.scss',
})
export class PartnerPortalNavMenuComponent {
  constructor(public authService: AdminAuthGuard, private router: Router) {}

  get currentRoute(): string {
    return this.router.url;
  }
  hideAdminTab: boolean = true;

  ngOnInit() {
    this.authService.hasAdminAccess().then((hasAccess) => {
      this.hideAdminTab = !hasAccess;
    });
  }
}
