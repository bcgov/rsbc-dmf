import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { EFormsServerOptions } from '../../../shared/components/phsa-form-viewer/phsa-form-viewer.component';
import { ConfigurationService } from '../../../shared/services/configuration.service';
import { AuthService } from '../../auth/services/auth.service';

@Component({
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss'],
})
export class ViewComponent implements OnInit {
  constructor(
    private configService: ConfigurationService,
    private route: ActivatedRoute,
    private AuthService: AuthService
  ) {}

  public ngOnInit(): void {
    return;
  }

  public serverOptions(): EFormsServerOptions {
    return this.configService.getEFormsServerOptions();
  }

  public formId(): string {
    return this.configService.getEFormsFormId('DMER');
  }

  public patientId(): string {
    return '123';
  }

  public practitionerId(): string {
    return this.AuthService.getHpdid();
    // if (this.loginService.userProfile?.clinics) {
    //   return this.loginService.userProfile?.clinics[0]?.practitionerId
    //     ? this.loginService.userProfile.clinics[0].practitionerId
    //     : '123';
    // } else {
    //   return '123';
    // }
  }

  public sessionId(): string {
    return this.AuthService.getHpdid(); //return keycloak user session here, return hepdid for now
  }
  public submissionId(): string {
    return this.route.snapshot.params['id'];
  }
}
