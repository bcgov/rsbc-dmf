import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  Input,
} from '@angular/core';
import { LoginService } from 'src/app/shared/services/login.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent {
  @Input() showProfile = false;

  showMobileMenu = false;

  public profileName?: string;
  public profileRole?: string;
  public profileInitials?: string;

  constructor(private loginService: LoginService) {
    const profile = this.loginService.userProfile;

    if (profile) {
      const firstName = profile.firstName;
      const lastName = profile.lastName;
      this.profileName = firstName + ' ' + lastName;
    }
  }

  public logOut(): void {
    this.loginService.logout();
  }

}
