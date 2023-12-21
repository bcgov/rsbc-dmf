/* tslint:disable */
/* eslint-disable */
import { EFormsOptions } from '../models/e-forms-options';
import { OidcOptions } from '../models/oidc-options';
export interface Configuration {
  eformsConfiguration?: EFormsOptions;
  environment?: string | null;
  oidcConfiguration?: OidcOptions;
}
