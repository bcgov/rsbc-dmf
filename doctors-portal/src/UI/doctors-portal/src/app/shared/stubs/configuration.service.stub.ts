import { Injectable } from '@angular/core';
import { EFormsServerOptions } from '../components/phsa-form-viewer/phsa-form-viewer.component';
import { ConfigurationService } from '../services/configuration.service';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationStubService extends ConfigurationService {

  public getEFormsServerOptions(): EFormsServerOptions {
    return {
      emrVendorId: 'testid',
      fhirServerUrl: 'fhirurl',
      formServerUrl: 'formurl'
    };
  }

}
