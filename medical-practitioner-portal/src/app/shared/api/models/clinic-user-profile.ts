/* tslint:disable */
/* eslint-disable */
import { ProviderRole } from '../models/provider-role';
export interface ClinicUserProfile {
  clinicId?: string | null;
  clinicName?: string | null;
  practitionerId?: string | null;
  role?: ProviderRole;
}
