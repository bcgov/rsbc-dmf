import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminAuthGuard } from '@app/modules/admin/admin.guard';
import { AuthGuard } from '@app/modules/keycloak/keycloak.guard';

@Component({
  selector: 'app-restricted-access',
  standalone: true,
  imports: [],
  templateUrl: './restricted-access.component.html',
  styleUrl: './restricted-access.component.css',
})
export class RestrictedAccessComponent {
  restriction: string = '';
  messageHeader: string = '';
  messageContent: string[] = [];
  constructor(
    private route: ActivatedRoute,
    public authService: AuthGuard,
    private router: Router,
  ) {}

  ngOnInit() {
    this.restriction = this.route.snapshot.paramMap.get('restriction') || '';
    if (this.restriction === 'unauthorizedUser') {
      this.authService.getCurrentUser().then((currentUser) => {
        if (currentUser !== null &&this.authService.hasUserAccess(currentUser) === true) {
          this.router.navigate(['']);
        }
      });
      this.messageHeader = 'Request Submitted';
      this.messageContent = [
        'Access request has been submitted - Approval Pending',
      ];
    } else if (this.restriction === 'expiredUser') {
      this.authService.getCurrentUser().then((currentUser) => {
        if (currentUser !== null &&this.authService.hasUserExpired(currentUser) === false) {
          this.router.navigate(['']);
        }
      });
      this.messageHeader = 'Access Restricted';
      this.messageContent = [
        'User profile has expired, please contact your system administrator to make changes',
      ];
    }
  }
}
