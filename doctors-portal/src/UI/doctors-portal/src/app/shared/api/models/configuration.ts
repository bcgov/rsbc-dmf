/* tslint:disable */
/* eslint-disable */
import { EFormsOptions } from './e-forms-options';
import { OidcOptions } from './oidc-options';

/**
 * Client configuration settings
 */
export interface Configuration {
  eformsConfiguration?: EFormsOptions;
  environment?: null | string;
  oidcConfiguration?: OidcOptions;
}
