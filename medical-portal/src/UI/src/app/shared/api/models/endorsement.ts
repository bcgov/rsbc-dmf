/* tslint:disable */
/* eslint-disable */
import { Licence } from '../models/licence';
export interface Endorsement {
  email?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  licence?: Array<Licence> | null;
  loginId?: string;
  role?: string | null;
  userId?: string | null;
}
