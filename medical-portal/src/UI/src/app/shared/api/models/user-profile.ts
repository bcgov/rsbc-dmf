/* tslint:disable */
/* eslint-disable */
import { Endorsement } from '../models/endorsement';
export interface UserProfile {
  email?: string | null;
  endorsements?: Array<Endorsement> | null;
  firstName?: string | null;
  id?: string | null;
  lastName?: string | null;
  loginId?: string | null;
  roles?: Array<string> | null;
}
