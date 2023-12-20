import { Component, ChangeDetectionStrategy, OnInit, Input } from '@angular/core';


@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderComponent {

  @Input() showProfile = false;

  public profileName?: string;
  public profileRole?: string
  public profileInitials?: string;

  constructor() {
    // const profile = this.loginService.userProfile;

    // if (profile) {
    //   const firstName = (profile.firstName || 'Not');
    //   const lastName = (profile.lastName || 'Available');
    //   this.profileName = firstName + ' ' + lastName;
    //   this.profileRole = profile.clinics?.map(c => `${c.role}`).join(',');
    //   this.profileInitials = firstName.substring(0, 1) + lastName.substring(0, 1);
    // }
  }


  public logOut(): void {
    //this.loginService.logout();
  }
}