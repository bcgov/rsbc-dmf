/* tslint:disable */
/* eslint-disable */
import { ClinicUserProfile } from './clinic-user-profile';
import { PractitionerBridge } from './practitioner-bridge';
export interface UserProfile {
  clinics?: null | Array<ClinicUserProfile>;
  emailAddress?: null | string;
  firstName?: null | string;
  id?: null | string;
  lastName?: null | string;
  practitioners?: null | Array<PractitionerBridge>;
}
