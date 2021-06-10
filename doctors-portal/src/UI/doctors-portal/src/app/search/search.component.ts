import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { FormServerOptions } from '../shared/components/phsa-form-viewer/phsa-form-viewer.component';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent {

  constructor() { }

  public serverOptions(): FormServerOptions {
    return environment.eformConfiguration;
  }
  public formId(): string { return '609eb6475d3fe59e856d8eea'; }
  public patientId(): string { return '123' };
  public practitionerId(): string { return '123' };
  public sessionId(): string { return 'session01' };
  public submissionId(): string { return '1' };
}
