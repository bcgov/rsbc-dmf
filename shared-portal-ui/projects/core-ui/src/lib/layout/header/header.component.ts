import { NgIf } from '@angular/common';
import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  Input,
  CUSTOM_ELEMENTS_SCHEMA,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import {MatMenuModule} from '@angular/material/menu';
import { RouterLink } from '@angular/router';
import { LoginService } from 'src/app/shared/services/login.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  imports:[RouterLink, MatIconModule, MatMenuModule, NgIf],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
  schemas:[CUSTOM_ELEMENTS_SCHEMA]
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
