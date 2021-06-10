import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-phsa-form-viewer',
  templateUrl: './phsa-form-viewer.component.html',
  styleUrls: ['./phsa-form-viewer.component.scss']
})
export class PhsaFormViewerComponent implements OnInit {

  @Input() formServerOptions: FormServerOptions | null = null;
  @Input() patientId: string | null = null;
  @Input() practitionerId: string | null = null;
  @Input() formId: string | null = null;
  @Input() submissionId: string | null = null;
  @Input() sessionId: string | null = null;

  constructor() { }

  public ngOnInit(): void {
    console.debug(this.formUrl());
    return;
  }

  public formUrl(): string {
    if (this.formServerOptions == null) throw Error('form server is not configured');
    return `${this.formServerOptions.formServerUrl}/Launch.html?` +
      `iss=${this.formServerOptions.fhirServerUrl}` +
      `&launch=${this.formServerOptions.emrVendorId}.${this.sessionId}.${this.patientId}.${this.practitionerId}.${this.formId}.${this.submissionId}`;
  }

}

export interface FormServerOptions {
  formServerUrl: string;
  fhirServerUrl: string;
  emrVendorId: string;
}
