/* tslint:disable */
/* eslint-disable */
import { Licence } from '../models/licence';
export interface Endorsement {
  email?: string | null;
  fullName?: string | null;
  licence?: Array<Licence> | null;
  loginId?: string;
  role?: string | null;
  userId?: string | null;
}
