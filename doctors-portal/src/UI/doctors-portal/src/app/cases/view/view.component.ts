import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EFormsServerOptions } from 'src/app/shared/components/phsa-form-viewer/phsa-form-viewer.component';
import { ConfigurationService } from 'src/app/shared/services/configuration.service';
import { LoginService } from 'src/app/shared/services/login.service';

@Component({
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {


  constructor(
    private configService: ConfigurationService,
    private route: ActivatedRoute,
    private loginService: LoginService
  ) { }

  public ngOnInit(): void {
    return;
  }

  public serverOptions(): EFormsServerOptions {
    return this.configService.getEFormsServerOptions();
  }
  public formId(): string { return this.configService.getEFormsFormId('DMER'); }
  public patientId(): string { return '123' };
  public practitionerId(): string { return '123' };
  public sessionId(): string { return this.loginService.getUserSession() };
  public submissionId(): string { return this.route.snapshot.params['id'] };
}
