import { AppEnvironment } from '../environments/environment.model';
import { InjectionToken, inject } from '@angular/core';

export const APP_CONFIG = new InjectionToken<AppConfig>('app.config');

export interface AppConfig extends AppEnvironment {
  routes: {
    auth: string;
    portal: string;
    admin: string;
  };
}
