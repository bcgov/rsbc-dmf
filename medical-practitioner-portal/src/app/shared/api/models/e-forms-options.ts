/* tslint:disable */
/* eslint-disable */
import { EFormDetails } from '../models/e-form-details';
export interface EFormsOptions {
  emrVendorId?: string | null;
  fhirServerUrl?: string | null;
  formServerUrl?: string | null;
  forms?: Array<EFormDetails> | null;
  formsMap?: string | null;
}
