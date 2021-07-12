import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Configuration } from '../api/models';
import { ConfigService } from '../api/services';
import { EFormsServerOptions } from '../components/phsa-form-viewer/phsa-form-viewer.component';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {
  private config: Configuration | null = null;

  constructor(private configurationService: ConfigService) { }

  public load(): Observable<Configuration> {
    if (this.config != null) {
      return of(this.config);
    }
    return this.configurationService.apiConfigGet$Json().pipe(
      tap((c) => {
        this.config = { ...c };
      })
    );
  }

  public getEFormsServerOptions(): EFormsServerOptions {
    if (!this.isConfigured() || !this.config?.eformsConfiguration) { throw Error('EForms server configuration is missing'); }
    return {
      emrVendorId: this.config.eformsConfiguration.emrVendorId || '',
      fhirServerUrl: this.config.eformsConfiguration.fhirServerUrl || '',
      formServerUrl: this.config.eformsConfiguration.formServerUrl || ''
    };
  }

  public getEFormsFormId(name: string): string {
    return this.config?.eformsConfiguration?.forms?.find(f => f.name?.toLowerCase() === name.toLowerCase())?.id || '';
  }

  public isConfigured(): boolean {
    return this.config !== null;
  }
}
