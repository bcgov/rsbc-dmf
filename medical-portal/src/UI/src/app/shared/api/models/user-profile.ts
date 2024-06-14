/* tslint:disable */
/* eslint-disable */
import { ClinicUserProfile } from '../models/clinic-user-profile';
import { PractitionerBridge } from '../models/practitioner-bridge';
export interface UserProfile {
  clinics?: Array<ClinicUserProfile> | null;
  emailAddress?: string | null;
  firstName?: string | null;
  id?: string | null;
  lastName?: string | null;
  practitioners?: Array<PractitionerBridge> | null;
}
