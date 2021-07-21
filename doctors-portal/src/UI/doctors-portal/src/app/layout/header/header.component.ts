import { Component, ChangeDetectionStrategy, OnInit, Input } from '@angular/core';
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
    if (this.loginService.isLoggedIn()) {
      const profile = loginService.getUserProfile();
      this.profileName = profile.firstName + ' ' + profile.lastName;
      this.profileRole = 'role';
      this.profileInitials = profile.firstName.substring(0, 1) + profile.lastName.substring(0, 1);
    }
  }

}
