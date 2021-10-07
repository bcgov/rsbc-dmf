import { Component, ChangeDetectionStrategy, OnInit, Input } from '@angular/core';
import { tap } from 'rxjs/operators';
import { LoginService } from 'src/app/shared/services/login.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderComponent {

  @Input() showProfile: boolean = false;

  public profileName?: string;
  public profileRole?: string
  public profileInitials?: string;

  constructor(private loginService: LoginService) {
    const profile = this.loginService.userProfile;

    if (profile) {
      const firstName = (profile.firstName || 'Not');
      const lastName = (profile.lastName || 'Available');
      this.profileName = firstName + ' ' + lastName;
      this.profileRole = profile.clinics?.map(c => `${c.role}@${c.clinicName}`).join(',');
      this.profileInitials = firstName.substring(0, 1) + lastName.substring(0, 1);
    }
  }


  public logOut(): void {
    this.loginService.logout();
  }
}
