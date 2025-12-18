import { Component, OnInit } from '@angular/core';
// common directives not needed here
import { PartnerPortalFooterComponent } from './Layout/partner-portal-footer/partner-portal-footer.component';
import { PartnerPortalHeaderComponent } from './Layout/partner-portal-header/partner-portal-header.component';
import { PartnerPortalNavMenuComponent } from './Layout/partner-portal-nav-menu/partner-portal-nav-menu.component';
import { Router, RouterOutlet } from '@angular/router';
import { ProfileService } from './shared/api/services';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: true,
  imports: [
    PartnerPortalFooterComponent,
    PartnerPortalHeaderComponent,
    PartnerPortalNavMenuComponent,
    RouterOutlet,
  ],
})
export class AppComponent implements OnInit
{
  constructor(

    private profileService : ProfileService,
    private router : Router
  ) {}
  ngOnInit() {

    this.profileService.apiProfileCurrentGet$Json().subscribe({
      next: (profile) => {
        console.info('User profile loaded:', profile);
      },
      error: (error) => {
        console.error('Error loading user profile:', error);
         if (error && error.status === 400) {
          // Handle unauthorized access by redirecting to the User Access Request page
          console.warn('Unauthorized access - redirecting to User Access Request page.');
          this.router.navigate(['/userAccess']);
        }
      }
    });
      
    console.info('AppComponent initialization completed.');
  }
}
