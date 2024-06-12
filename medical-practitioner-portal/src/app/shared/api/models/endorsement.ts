/* tslint:disable */
/* eslint-disable */
import { Licence } from '../models/licence';
export interface Endorsement {
  licence?: Array<Licence> | null;
  userId?: string | null;
}
