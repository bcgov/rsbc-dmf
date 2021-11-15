import { Component, OnInit } from '@angular/core';
import { ClinicUserProfile } from 'src/app/shared/api/models';
import { LoginService } from 'src/app/shared/services/login.service';

@Component({
  selector: 'app-clinics',
  templateUrl: './clinics.component.html',
  styleUrls: ['./clinics.component.scss']
})
export class ClinicsComponent implements OnInit {

  myClinics?: null | Array<ClinicUserProfile> = [];

  constructor(private loginService: LoginService) {
    const profile = this.loginService.userProfile;

    if (profile)
    {
      this.myClinics = profile.clinics;      
    }
      
  }

  ngOnInit(): void {
  }

}
