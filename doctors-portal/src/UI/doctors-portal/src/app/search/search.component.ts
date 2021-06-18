import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { EFormsServerOptions } from '../shared/components/phsa-form-viewer/phsa-form-viewer.component';
import { ConfigurationService } from '../shared/services/configuration.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent {

  constructor(private configService: ConfigurationService) { }

  public serverOptions(): EFormsServerOptions {
    return this.configService.getEFormsServerOptions();
  }
  public formId(): string { return '609eb617894b2ab618b917ac'; }
  public patientId(): string { return '123' };
  public practitionerId(): string { return '123' };
  public sessionId(): string { return 'session01' };
  public submissionId(): string { return '1' };
}
