/* eslint-disable @typescript-eslint/explicit-member-accessibility */
import { Component, OnInit } from '@angular/core';

import { ClinicUserProfile } from '../../../shared/api/models';

@Component({
  selector: 'app-clinics',
  templateUrl: './clinics.component.html',
  styleUrls: ['./clinics.component.scss'],
})
export class ClinicsComponent {
  myClinics?: null | Array<ClinicUserProfile> = [];
  // constructor(private loginService: LoginService) {
  //   const profile = this.loginService.userProfile;
  //   if (profile) {
  //     this.myClinics = profile.clinics;
  //   }
  // }
}
